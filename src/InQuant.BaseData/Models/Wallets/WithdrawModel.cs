namespace InQuant.BaseData.Models.Wallets
{
    public class WithdrawModel
    {
        /// <summary>
        /// 钱包系统的提现申请ID，参数传入值
        /// </summary>
        public int Withdrawid { get; set; }

        /// <summary>
        /// 交易状态。0 进行中；1 成功；2 失败
        /// </summary>
        public string Txstatus { get; set; }

        /// <summary>
        /// 失败原因。txstatus=2时显示。
        /// </summary>
        public string Failreason { get; set; }

        /// <summary>
        /// 链上的交易ID。 如果还没有发到链上或者发到链上时失败了，这个字段为空
        /// </summary>
        public string Txid { get; set; }

        /// <summary>
        /// 这笔交易的入金地址
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// 这笔交易的金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 出金请求的最后更新时间。UTC时间，精确到毫秒
        /// </summary>
        public long Time { get; set; }
    }
}
