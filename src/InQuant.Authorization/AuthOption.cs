using System.Collections.Generic;

namespace InQuant.Authorization
{
    public class AuthOption
    {
        /// <summary>
        /// 过期时间HH:MM:SS
        /// </summary>
        public string TokenExpireTime { get; set; } = "00:30:00";

        public string CookieDomain { get; set; }

        /// <summary>
        /// ip白名单 (配置WhiteListFilterAttribute)
        /// </summary>
        public List<string> IpWhiteList { get; set; }

        /// <summary>
        /// 用户系统，user token redis key preffix
        /// </summary>
        public string UserTokenCacheKeyPreffix { get; set; } = "hopex:usersystem:";
    }
}
