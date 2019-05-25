using Microsoft.Extensions.DependencyInjection;

namespace InQuant.MQ
{
    public static class IServiceCollectionExtensions
    {
        public static void AddMQ(this IServiceCollection services)
        {
            services.AddHostedService<MessageManager>();
        }
    }
}
