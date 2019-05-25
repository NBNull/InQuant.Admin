using InQuant.Framework.Data.Core.Sessions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace InQuant.Framework.Utils
{
    /// <summary>
    /// 无缓冲工作池
    /// </summary>
    public class NoBufferPoolWorker
    {
        private readonly int _max_pool_size;
        private readonly ILogger _logger;
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);
        private readonly ConcurrentDictionary<Task, bool> _pool = new ConcurrentDictionary<Task, bool>();

        public NoBufferPoolWorker(int maxPoolSize, ILogger logger)
        {
            _max_pool_size = maxPoolSize;
            _logger = logger;
        }

        public async Task QueueWorkItem(Action action)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                while (_pool.Count >= _max_pool_size)
                {
                    //_logger.LogInformation("pool 已满，等待100ms");

                    await Task.Delay(100);//如果pool已满，等待100ms
                }

                var t = new TaskFactory().StartNew(action);

                _pool.TryAdd(t, true);

                var _ = t.ContinueWith((ts) =>
                  {
                      _pool.TryRemove(ts, out bool _);
                      //_logger.LogInformation("work dequeued. cur size: {size}", _pool.Count);
                  });

                //_logger.LogInformation("work queued. cur size: {size}", _pool.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "QueueWorkItem fail");
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task QueueWorkItem(Action<IServiceScope> action, IServiceProvider serviceProvider)
        {
            await QueueWorkItem(() =>
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var sessionContext = scope.ServiceProvider.GetRequiredService<IDapperSessionContext>();
                    try
                    {
                        action(scope);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "执行失败");
                        sessionContext.Cancel();
                    }
                }
            });
        }
    }
}
