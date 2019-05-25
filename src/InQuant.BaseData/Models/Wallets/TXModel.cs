namespace InQuant.BaseData.Models.Wallets
{
    /// <summary>
    /// 钱包返回的交易记录
    /// </summary>
    public class TXModel
    {
        /// <summary>
        /// 入金ID
        /// </summary>
        public string Depositid { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Assetcode { get; set; }

        /// <summary>
        /// 交易状态。0 进行中；1 成功；2 失败
        /// </summary>
        public string Txstatus { get; set; }

        /// <summary>
        /// 失败原因。txstatus=2时显示
        /// </summary>
        public string Failreason { get; set; }

        /// <summary>
        /// 链上的交易ID。 如果还没有发到链上或者发到链上时失败了（出金），这个字段为空
        /// </summary>
        public string Txid { get; set; }

        /// <summary>
        /// 这笔交易的出金地址。如果from==address，表示是出金
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// 这笔交易的入金地址。如果to==address，表示是入金
        /// </summary>
        public string To { get; set; }

        /// <summary>
        /// 这笔交易的金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 入金为链上的时间（具体表示的时间待定）；出金为出金申请最后更新时间。UTC时间，精确到毫秒
        /// </summary>
        public long Time { get; set; }
    }
}
