using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InQuant.MQ
{
    public class MessageManager : IHostedService
    {
        private bool disposed = false;
        private readonly ILogger _logger;
        private readonly List<Task> _listenTask;
        private readonly List<IMessageConsumer> _consumers;
        private readonly ISubscribeProvider _subscribeProvider;
        private readonly IManualOffsetProvider _manualOffsetProvider;
        private readonly CancellationTokenSource _tokenSource;

        public MessageManager(ILogger<MessageManager> logger,
            ISubscribeProvider subscribeProvider,
            IManualOffsetProvider manualOffsetProvider,
            IApplicationLifetime applicationLifetime,
            IEnumerable<IMessageConsumer> consumers)
        {
            _tokenSource = new CancellationTokenSource();

            _logger = logger;
            _subscribeProvider = subscribeProvider;
            _manualOffsetProvider = manualOffsetProvider;
            _listenTask = new List<Task>();
            _consumers = consumers.ToList();
            applicationLifetime.ApplicationStopping.Register(Dispossing);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("开始监听并处理kafka消息，共{TC}个consumer...", _consumers.Count);

            foreach (var consumer in _consumers)
            {
                if (consumer.Type == ConsumerType.ManaualOffset)
                {
                    _manualOffsetProvider.Scribe(consumer);
                }
                else
                {
                    _subscribeProvider.Scribe(consumer);
                }
            }

            _listenTask.AddRange(_manualOffsetProvider.Listen(_tokenSource.Token));
            _listenTask.AddRange(_subscribeProvider.Listen(_tokenSource.Token));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Dispossing();
            _logger.LogInformation("消息服务已停止");

            return Task.CompletedTask;
        }

        private void Dispossing()
        {
            if (disposed) return;

            disposed = true;

            try
            {
                _listenTask.Clear();
                _tokenSource.Cancel();
            }
            catch { }

            _tokenSource.Dispose();
        }
    }
}
