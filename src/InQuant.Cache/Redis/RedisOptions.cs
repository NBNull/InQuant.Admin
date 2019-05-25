namespace InQuant.Cache.Redis
{
    public class RedisOptions
    {
        public string[] Servers { get; set; }

        public string ServiceName { get; set; } = "mymaster";

        public string KeyPreffix { get; set; }
    }
}
