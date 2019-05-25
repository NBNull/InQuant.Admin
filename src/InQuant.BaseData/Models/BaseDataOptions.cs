namespace InQuant.BaseData.Models
{
    /// <summary>
    /// basedata模块配置文件
    /// </summary>
    public class BaseDataOptions
    {
        ///// <summary>
        ///// 文件根目录
        ///// </summary>
        //public string FileRootDirctory { get; set; }

        /// <summary>
        /// 钱包api base url
        /// </summary>
        public string WalletBaseUrl { get; set; } = "http://api.bitinfi.com";

        /// <summary>
        /// hopex在钱包平台的app id
        /// </summary>
        public string WalletAppId { get; set; }

        /// <summary>
        /// hopex在钱包平台的app key
        /// </summary>
        public string WalletAppKey { get; set; }

        /// <summary>
        /// 钱包des加密 key
        /// </summary>
        public string WalletDesKey { get; set; } = "shenzhen12345678shenzhen";

        /// <summary>
        /// 钱包des加密 iv
        /// </summary>
        public string WalletDesIV { get; set; } = "12312300";

        /// <summary>
        /// 获取新的热入地址
        /// </summary>
        public string Getnewaddress { get; set; } = "/wallet/getnewaddress.ashx";

        /// <summary>
        /// 查询出金申请状态（单个或多个）
        /// </summary>
        public string GetWithdrawList { get; set; } = "/wallet/getwithdrawlist.ashx";

        /// <summary>
        /// 根据address查所有交易记录
        /// </summary>
        public string GetTXList { get; set; } = "/wallet/gettxlist.ashx";

        /// <summary>
        /// 根据AppId查所有交易记录
        /// </summary>
        public string GetAppidTXlist { get; set; } = "/wallet/getappidtxlist.ashx";

        /// <summary>
        /// 出金请求
        /// </summary>
        public string Withdraw { get; set; } = "/wallet/withdrawapply.ashx";

        /// <summary>
        /// asset群，webhook地址
        /// </summary>
        public string AssetGroupWebHook { get; set; } = "https://oapi.dingtalk.com/robot/send?access_token=971c479f2426077ce72408d0760e9b48274c132649b9e64044bc6aa9eb4bb5a0";

        /// <summary>
        /// 提现手续费账户ID
        /// </summary>
        public int WithdrawCommissionUserId { get; set; } = 9;

        /// <summary>
        /// 保险基金账户ID
        /// </summary>
        public int InsurFundUserId { get; set; } = 1;

        /// <summary>
        /// 交易手续费账户ID
        /// </summary>
        public int TradeCommissionUserId { get; set; } = 2;

        /// <summary>
        /// 作市账户ID
        /// </summary>
        public int[] MarketRobotUserId { get; set; } = new int[0];

        /// <summary>
        /// 风控账户ID
        /// </summary>
        public int[] RiskControlUserId { get; set; } = new int[0];

        /// <summary>
        /// api host
        /// </summary>
        public string ApiHost { get; set; } = "https://api.hopex.com";

        /// <summary>
        /// 用户系统host
        /// </summary>
        public string UserHost { get; set; } = "https://user.hopex.com";

        /// <summary>
        /// BusinessCall系统 cache key前缀
        /// </summary>
        public string BusinessCallCachePreffix { get; set; } = "hopex:business_call:";

        /// <summary>
        /// 后台任务通知，钉钉机器人地址
        /// </summary>
        public string BackgroundJobNotifyWebHook { get; set; } = "https://oapi.dingtalk.com/robot/send?access_token=a2d9b0509fb793b8f477f801f494f01c1533d5ced8863fc5cba532794c1a736e";
    }
}
