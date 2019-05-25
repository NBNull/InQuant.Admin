using InQuant.DingTalk.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InQuant.DingTalk
{
    public static class IServiceCollectionExtensions
    {
        public static void AddDingTalk(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DingTalkOption>(ops => configuration.GetSection("DingTalk").Bind(ops));
        }

    }
}
