using InQuant.Framework.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace InQuant.Framework.Extensions
{
    public static class HttpContextExtensions
    {
        /// <summary>
        /// 获取当前请求的客户端设备类型
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static PackTypeEnum GetPackType(this HttpRequest request)
        {
            var heads = request.Headers;
            if (heads.TryGetValue("packType", out StringValues t))
            {
                return ((string)t).ToPackType() ?? PackTypeEnum.PcWeb;
            }
            return PackTypeEnum.PcWeb;
        }

    }
}
