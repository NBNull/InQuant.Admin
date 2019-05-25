using InQuant.Kafka.Models;
using InQuant.Kafka.Services.Impl;
using InQuant.MQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InQuant.Kafka
{
    public static class IServiceCollectionExtensions
    {
        public static void AddKafka(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<KafkaOption>(x => configuration.GetSection("kafka").Bind(x));
            services.AddSingleton<IProducer, Producer>();
            services.AddSingleton<ISubscribeProvider, KafkaScribeProvider>();
            services.AddSingleton<IManualOffsetProvider, KafkaManualOffsetProvider>();
        }
    }
}
