using System;

namespace InQuant.Kafka.Models
{
    public class KafkaOption
    {
        /// <summary>
        /// kafka servers
        /// </summary>
        public string Servers { get; set; } = "192.168.70.131:9092";

        public string ConsumerGroupId { get; set; } = "hopex.admin";
    }
}
