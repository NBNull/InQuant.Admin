namespace InQuant.BaseData.Models
{
    public class ApiConsts
    {
        /// <summary>
        /// 获取单个币对的行情
        /// </summary>
        public const string SingleGetQuotation = "/v1/ticker/single";

        /// <summary>
        /// 获取多个币对的行情
        /// </summary>
        public const string MultiGetQuotation = "/v1/ticker/multi";

        /// <summary>
        /// 获取上架的banner列表
        /// </summary>
        public const string GetOnShelvesBanners = "/api/operation/banner/GetOnShelvesBanners?type={0}";

        /// <summary>
        /// 获取上架的公告列表
        /// </summary>
        public const string GetOnShelvesNotifies = "/api/operation/notify/GetOnShelvesNotifies";
    }
}
