using InQuant.Framework.Exceptions;
using InQuant.Framework.Utils;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace InQuant.Authorization
{
    public class WhiteListFilterAttribute : ActionFilterAttribute
    {
        private readonly AuthOption _options;

        public WhiteListFilterAttribute(IOptions<AuthOption> options)
        {
            _options = options.Value;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string ip = IpUtil.GetClientIp(context.HttpContext);

            if (string.IsNullOrWhiteSpace(ip))
                throw new HopexException($"无法取到来源IP，不允许访问");

            if (_options.IpWhiteList == null || _options.IpWhiteList.Count == 0)
                throw new HopexException("无法读取IP白名单配置");

            if (!_options.IpWhiteList.Contains(ip))
                throw new HopexException($"IP[{ip}]无权范围");

            await next.Invoke();
        }
    }
}
