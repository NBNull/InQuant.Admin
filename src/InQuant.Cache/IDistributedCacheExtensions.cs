using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InQuant.Cache
{
    public static class IDistributedCacheExtensions
    {
        readonly static string _null_value = "!null_value";

        private readonly static ConcurrentDictionary<string, SemaphoreSlim> _lockDict = new ConcurrentDictionary<string, SemaphoreSlim>();
        private static SemaphoreSlim GetLock(string name)
        {
            return _lockDict.GetOrAdd(name, new SemaphoreSlim(1, 1));
        }

        public static async Task<T> GetAsync<T>(this IDistributedCache cache, string key, Func<Task<T>> getData, TimeSpan? expire = null, CancellationToken token = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));

            if (getData == null) throw new ArgumentNullException(nameof(getData));

            token.ThrowIfCancellationRequested();

            var bytes = await cache.GetAsync(key, token).ConfigureAwait(false);

            if (bytes == null)
            {
                var m = await getData();
                if (m != null)
                {
                    var options = new DistributedCacheEntryOptions();
                    if (expire != null)
                        options.AbsoluteExpirationRelativeToNow = expire;

                    await cache.SetStringAsync(key, JsonConvert.SerializeObject(m), options, token).ConfigureAwait(false);
                }
                return m;
            }

            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(bytes));
        }
        
        public static async Task Set<T>(this IDistributedCache cache, string key, T data, TimeSpan? expireIn = null, CancellationToken token = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            token.ThrowIfCancellationRequested();

            string jsonData = JsonConvert.SerializeObject(data);
            var options = new DistributedCacheEntryOptions();
            if (expireIn != null)
            {
                options.AbsoluteExpirationRelativeToNow = expireIn.Value;
            };
            await cache.SetStringAsync(key, jsonData, options, token);
        }

        public static async Task SetSlidingExpiration<T>(this IDistributedCache cache, string key, T data, TimeSpan? expireIn = null, CancellationToken token = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            token.ThrowIfCancellationRequested();

            string jsonData = JsonConvert.SerializeObject(data);
            var options = new DistributedCacheEntryOptions();
            if (expireIn != null)
            {
                options.SlidingExpiration = expireIn;
            };
            await cache.SetStringAsync(key, jsonData, options, token);
        }

        public static async Task<T> GetAsync<T>(this IMemoryCache cache, string key, Func<Task<T>> getData, TimeSpan? expire = null, CancellationToken token = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));
            if (getData == null) throw new ArgumentNullException(nameof(getData));

            token.ThrowIfCancellationRequested();

            if (!cache.TryGetValue(key, out string str) || string.IsNullOrWhiteSpace(str))
            {
                var @lock = GetLock(key);
                try
                {
                    await @lock.WaitAsync();

                    if (!cache.TryGetValue(key, out str) || string.IsNullOrWhiteSpace(str))
                    {
                        var m = await getData();
                        if (m != null)
                        {
                            using (var entry = cache.CreateEntry(key))
                            {
                                entry.SetValue(JsonConvert.SerializeObject(m));

                                if (expire != null)
                                    entry.SetAbsoluteExpiration(DateTime.Now.Add(expire.Value));
                            }
                        }
                        else
                        {
                            //如果是null，缓存一个特殊的字符串，缓存15秒，        
                            using (var entry = cache.CreateEntry(key))
                            {
                                entry.SetValue(_null_value);

                                if (expire != null)
                                    entry.SetAbsoluteExpiration(DateTime.Now.Add(TimeSpan.FromSeconds(15)));
                            }
                        }
                        return m;
                    }
                }
                finally
                {
                    @lock.Release();
                }
            }

            if (str == _null_value) return default(T);

            return JsonConvert.DeserializeObject<T>(str);
        }
    }
}