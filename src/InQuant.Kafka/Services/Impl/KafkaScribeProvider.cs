using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using InQuant.Framework.Data.Core.Sessions;
using InQuant.Kafka.Models;
using InQuant.MQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InQuant.Kafka.Services.Impl
{
    public class KafkaScribeProvider : ISubscribeProvider
    {
        private readonly ILogger _logger;
        private readonly KafkaOption _kafkaOption;
        private readonly IServiceProvider _serviceProvider;

        private readonly Dictionary<string, List<IMessageConsumer>> _consumers;

        public KafkaScribeProvider(IOptions<KafkaOption> kafkaOption,
            IServiceProvider serviceProvider,
            ILogger<KafkaScribeProvider> logger)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _kafkaOption = kafkaOption.Value;

            _consumers = new Dictionary<string, List<IMessageConsumer>>();
        }

        public List<Task> Listen(CancellationToken token)
        {
            if (_consumers.Count == 0) return new List<Task>();

            List<Task> ts = new List<Task>();
            foreach (var topic in _consumers.Keys)
            {
                ts.Add(Task.Factory.StartNew(() =>
                {
                    try
                    {
                        ListenSingle(topic, token);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"{topic} listen出错");
                    }
                }, TaskCreationOptions.LongRunning));
            }

            return ts;
        }

        public void ListenSingle(string topic, CancellationToken token)
        {
            _logger.LogInformation("listen {topic},by subscribe", topic);

            var config = new Dictionary<string, object>
            {
                { "bootstrap.servers", _kafkaOption.Servers },
                { "group.id", _kafkaOption.ConsumerGroupId },
                { "enable.auto.commit", false },
                { "statistics.interval.ms", 60000 },
                { "session.timeout.ms", 6000 },
                { "auto.offset.reset", "smallest" }
            };

            using (var consumer = new Consumer<string, string>(config,
                new StringDeserializer(Encoding.UTF8), new StringDeserializer(Encoding.UTF8)))
            {
                consumer.OnPartitionEOF += (_, end)
                    => _logger.LogDebug($"end of topic {end.Topic} partition {end.Partition}, next message will be at offset {end.Offset}");

                consumer.OnError += (_, error)
                    => _logger.LogError($"Error: {error}, topc: {topic}");

                // Raised on deserialization errors or when a consumed message has an error != NoError.
                consumer.OnConsumeError += (_, msg)
                    => _logger.LogError($"Error consuming from topic/partition/offset {msg.Topic}/{msg.Partition}/{msg.Offset}: {msg.Error}");

                consumer.OnOffsetsCommitted += (_, commit) =>
                {
                    if (commit.Error) _logger.LogError($"Failed to commit offsets: {commit.Error}");
                    else _logger.LogInformation($"Successfully committed offsets: [{string.Join(", ", commit.Offsets)}]");
                };

                // Raised when the consumer is assigned a new set of partitions.
                consumer.OnPartitionsAssigned += (_, partitions) =>
                {
                    _logger.LogInformation($"Assigned partitions: [{string.Join(", ", partitions)}], member id: {consumer.MemberId}");
                    // If you don't add a handler to the OnPartitionsAssigned event,
                    // the below .Assign call happens automatically. If you do, you
                    // must call .Assign explicitly in order for the consumer to 
                    // start consuming messages.
                    consumer.Assign(partitions);
                };

                // Raised when the consumer's current assignment set has been revoked.
                consumer.OnPartitionsRevoked += (_, partitions) =>
                {
                    _logger.LogInformation($"Revoked partitions: [{string.Join(", ", partitions)}]");
                    // If you don't add a handler to the OnPartitionsRevoked event,
                    // the below .Unassign call happens automatically. If you do, 
                    // you must call .Unassign explicitly in order for the consumer
                    // to stop consuming messages from it's previously assigned 
                    // partitions.
                    consumer.Unassign();
                };

                //consumer.OnStatistics += (_, json)
                //    => _logger.LogInformation($"Statistics: {json}, topc: {topic}");

                consumer.Subscribe(topic);

                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        if (!consumer.Consume(out Message<string, string> msg, TimeSpan.FromMilliseconds(100)))
                        {
                            continue;
                        }

                        _logger.LogDebug("consumer {topic} key: {key}, message: {msg}, offset: {offset}", msg.Topic, msg.Key, msg.Value, msg.Offset);

                        Task<CommittedOffsets> offsets_t = null;

                        if (msg.Offset % 5 == 0)
                        {
                            offsets_t = consumer.CommitAsync(msg);
                        }
                        
                        new TaskFactory().StartNew(() => Process(topic, msg.Key, msg.Value)).Unwrap().GetAwaiter().GetResult();
                        //Process(topic, msg.Key, msg.Value).ConfigureAwait(false).GetAwaiter().GetResult();

                        if (offsets_t != null) offsets_t.GetAwaiter().GetResult();
                    }
                    catch (KafkaException e)
                    {
                        _logger.LogError(e, $"{topic} poll error: {e.Error?.Reason}");
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.LogInformation($"{topic} poll Operation Canceled");
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"{topic} poll Error：{e.Message}");
                    }
                }

                try
                {
                    //force commit offset
                    var _ = consumer.CommitAsync().Result;
                }
                catch { }
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

        public void Scribe(IMessageConsumer consumer)
        {
            if (consumer == null) throw new ArgumentNullException(nameof(consumer));
            if (string.IsNullOrWhiteSpace(consumer.Topic)) throw new Exception($"{consumer.GetType()} topic为空");
            if (consumer.Type != ConsumerType.Subscribe) throw new Exception($"{consumer.GetType()} 不是subscribe模式的消费者");

            if (!_consumers.ContainsKey(consumer.Topic))
            {
                _consumers.Add(consumer.Topic, new List<IMessageConsumer>() { consumer });
            }
            else
            {
                _consumers[consumer.Topic].Add(consumer);
            }
        }
    }
}
