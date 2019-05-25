using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using InQuant.Kafka.Models;
using InQuant.MQ;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InQuant.Kafka.Services.Impl
{
    public class Producer : IProducer
    {
        private readonly ILogger _logger;
        private readonly Dictionary<string, object> _config;
        private Lazy<Producer<string, string>> _producer;

        public Producer(IOptions<KafkaOption> option, ILogger<Producer> logger)
        {
            _config = new Dictionary<string, object> {
                { "bootstrap.servers", option.Value.Servers },
                { "queue.buffering.max.ms", 0 }
            };
            _producer = new Lazy<Producer<string, string>>(() => new Producer<string, string>(_config, new StringSerializer(Encoding.UTF8), new StringSerializer(Encoding.UTF8)));
            _logger = logger;
        }

        public void Dispose()
        {
            try
            {
                _logger.LogInformation("dispossing producer....");

                if (_producer.IsValueCreated)
                {
                    _producer.Value.Dispose();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "dispose producer fail");
            }
        }

        public async Task Produce<T>(string topic, string key, T m)
        {
            await _producer.Value.ProduceAsync(topic, key, JsonConvert.SerializeObject(m));

        }

        public async Task Produce<T>(string topic, T m)
        {
            await _producer.Value.ProduceAsync(topic, null, JsonConvert.SerializeObject(m));
        }
    }
}
