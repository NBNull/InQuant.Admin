using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using InQuant.Cache.Redis;
using InQuant.Framework.Data.Core.Sessions;
using InQuant.Kafka.Models;
using InQuant.MQ;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InQuant.Kafka.Services.Impl
{
    public class KafkaManualOffsetProvider : IManualOffsetProvider, IDisposable
    {
        const string OFFSET_CACHE = "notify:{0}";
        private bool disposed = false;

        private readonly RedisOptions _redisOptions;
        private readonly ILogger _logger;
        private readonly KafkaOption _kafkaOption;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDistributedCache _distributedCache;

        private readonly Dictionary<string, List<IMessageConsumer>> _consumers;

        private readonly ConcurrentDictionary<string, long> _offset;
        private readonly ConcurrentDictionary<string, long> _pre_offset;
        private readonly Timer _timer;

        public KafkaManualOffsetProvider(IOptions<KafkaOption> kafkaOption,
            IServiceProvider serviceProvider,
            IOptions<RedisOptions> redisOption,
            IDistributedCache distributedCache,
            ILogger<KafkaScribeProvider> logger)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _kafkaOption = kafkaOption.Value;
            _redisOptions = redisOption.Value;
            _distributedCache = distributedCache;

            _consumers = new Dictionary<string, List<IMessageConsumer>>();
            _offset = new ConcurrentDictionary<string, long>();
            _pre_offset = new ConcurrentDictionary<string, long>();

            _timer = new Timer(SaveOffset, null, TimeSpan.Zero, TimeSpan.FromSeconds(3));
        }

        private void SaveOffset(object state)
        {
            bool changed = false;
            List<Task> ts = new List<Task>();

            foreach (var (topic, offset) in _offset)
            {
                changed = false;
                if (!_pre_offset.TryGetValue(topic, out long pre_offset))
                {
                    _pre_offset.TryAdd(topic, offset);
                    changed = true;
                }
                else
                {
                    if (offset != pre_offset)
                    {
                        _pre_offset.TryUpdate(topic, offset, pre_offset);
                        changed = true;
                    }
                }
                if (changed) ts.Add(_distributedCache.SetStringAsync(string.Format(OFFSET_CACHE, topic), offset.ToString()));
            }

            if (ts.Count > 0) Task.WhenAll(ts).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private List<TopicPartitionOffset> GetAllTopicPartitionOffset()
        {
            var pipe = RedisHelper.StartPipe();

            foreach (var topic in _consumers.Keys)
                pipe.HMGet($"{_redisOptions.KeyPreffix}{string.Format(OFFSET_CACHE, topic)}", "data");

            var vals = pipe.EndPipe();

            var poffsets = new List<TopicPartitionOffset>();

            for (int i = 0; i < vals.Length; i++)
            {
                var val = ((string[])vals[i]).FirstOrDefault();
                var topic = _consumers.Keys.ElementAt(i);

                long offset = !string.IsNullOrWhiteSpace(val) ? long.Parse(val) : -1;
                poffsets.Add(new TopicPartitionOffset(topic, 0, offset == -1 ? Offset.End : new Offset(offset + 1)));
            }

            return poffsets;
        }

        public List<Task> Listen(CancellationToken token)
        {
            if (_consumers.Count == 0) return new List<Task>();

            List<TopicPartitionOffset> poffsets = GetAllTopicPartitionOffset();
            List<Task> ts = new List<Task>();

            foreach (var topicOffset in poffsets)
            {
                ts.Add(Task.Factory.StartNew(() =>
                {
                    try
                    {
                        ListenSingle(topicOffset, token);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"{topicOffset.Topic} listen出错");
                    }
                }, TaskCreationOptions.LongRunning));
            }

            return ts;
        }

        public void ListenSingle(TopicPartitionOffset topicOffset, CancellationToken token)
        {
            _logger.LogInformation("listen {topic},by manual offset, offset: {offset}", topicOffset.Topic, topicOffset.Offset);

            string topic = topicOffset.Topic;

            var config = new Dictionary<string, object>
            {
                { "bootstrap.servers", _kafkaOption.Servers },
                { "group.id", new Guid().ToString() },
                { "enable.auto.commit", false }
            };

            using (var consumer = new Consumer<string, string>(config,
                new StringDeserializer(Encoding.UTF8), new StringDeserializer(Encoding.UTF8)))
            {
                consumer.Assign(new[] { topicOffset });

                consumer.OnError += (_, error) => _logger.LogError($"Error: {error}, topic: {topicOffset.Topic}");

                consumer.OnConsumeError += (_, error) => _logger.LogError($"Consume error: {error}, topic: {topicOffset.Topic}");

                while (true)
                {
                    try
                    {
                        if (token.IsCancellationRequested) break;

                        if (consumer.Consume(out Message<string, string> cr, TimeSpan.FromSeconds(1)))
                        {
                            _logger.LogDebug("consumer {topic} message: {msg}, offset: {offset}", cr.Topic, cr.Value, cr.Offset);

                            _offset.AddOrUpdate(cr.Topic, cr.Offset, (t, o) => cr.Offset);

                            new TaskFactory().StartNew(() => Process(cr.Topic, cr.Key, cr.Value)).Unwrap().GetAwaiter().GetResult();
                        }
                    }
                    catch (KafkaException e)
                    {
                        _logger.LogError(e, $"{topic} consumer Error: {e.Error?.Reason}");
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogInformation("{topicOffset.Topic} consumer Operation Canceled");
                        break;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"{topic} consumer Error：{e.Message}");
                    }
                }
            }
        }

        private async Task Process(string topic, string key, string message)
        {
            if (_consumers.TryGetValue(topic, out List<IMessageConsumer> consumers))
            {
                var ts = new List<Task>();

                foreach (var consumer in consumers) ts.Add(SaftyProcess(consumer, key, message));

                foreach (var t in ts) await t;
            }
        }

        private async Task SaftyProcess(IMessageConsumer consumer, string key, string message)
        {
            if (consumer.IgnoreKey(key)) return;

            using (var scope = _serviceProvider.CreateScope())
            {
                var sessionContext = scope.ServiceProvider.GetRequiredService<IDapperSessionContext>();
                try
                {
                    await consumer.Handler(scope, key, message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"{consumer.GetType()} 消费消息出错，{consumer.Topic}-{key}-{message}");
                    sessionContext.Cancel();
                }
            }
        }

        private async Task ProcessSingle(Func<IServiceScope, string, string, Task> f, string topic, string message)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                await f(scope, topic, message);
            }
        }

        public void Scribe(IMessageConsumer consumer)
        {
            if (consumer == null) throw new ArgumentNullException(nameof(consumer));
            if (string.IsNullOrWhiteSpace(consumer.Topic)) throw new Exception($"{consumer.GetType()} topic为空");
            if (consumer.Type != ConsumerType.ManaualOffset) throw new Exception($"{consumer.GetType()} 不是ManaualOffset模式的消费者");

            if (!_consumers.ContainsKey(consumer.Topic))
            {
                _consumers.Add(consumer.Topic, new List<IMessageConsumer>() { consumer });
            }
            else
            {
                _consumers[consumer.Topic].Add(consumer);
            }
        }

        public void Dispose()
        {
            Disposing();
        }

        private void Disposing()
        {
            if (disposed)
                return;

            try
            {
                SaveOffset(null);
                _logger.LogInformation("kafka offset saved.");
            }
            catch { }

            _timer?.Change(Timeout.Infinite, 0);
            _timer?.Dispose();

            disposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
