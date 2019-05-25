using InQuant.Cache.Redis;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace InQuant.BaseData.Services.Impl
{
    public class NoGenerator : INoGenerator
    {
        private readonly RedisOptions _redisOptions;

        public NoGenerator(IOptions<RedisOptions> redisOptions)
        {
            _redisOptions = redisOptions.Value;
        }

        public async Task<string> NextNo(string category, byte incrementBit = 4)
        {
            string date = DateTime.Now.Date.ToString("yyyyMMdd");
            string key = $"{_redisOptions.KeyPreffix}{category}:{date}";
            var expires = DateTime.Now.Date.AddDays(1) - DateTime.Now.Date;

            var id = (long)await RedisHelper.EvalAsync(@"
                        local result = redis.call('incr', KEYS[1])
                        redis.call('EXPIRE', KEYS[1], ARGV[1])
                        return result",
                    key, (long)expires.TotalSeconds);

            string ids = id.ToString();
            if (ids.Length < incrementBit)
            {
                ids = id.ToString($"d{incrementBit}");
            }

            return $"{category}{date}{ids}";
        }

    }
}
