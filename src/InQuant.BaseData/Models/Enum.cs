using System.ComponentModel;

namespace InQuant.BaseData.Models
{
    /// <summary>
    /// 标的币种
    /// </summary>
    public enum UnderlyingCurrency
    {
        /// <summary>
        /// 比特币
        /// </summary>
        [Description("比特币")]
        BTC,

        /// <summary>
        /// 以太坊
        /// </summary>
        [Description("以太坊")]
        ETH,

        /// <summary>
        /// 瑞波币
        /// </summary>
        [Description("瑞波币")]
        XRP,

        /// <summary>
        /// 柚子币
        /// </summary>
        [Description("柚子币")]
        EOS,

        /// <summary>
        /// 莱特币 
        /// </summary>
        [Description("莱特币")]
        LTC,

        /// <summary>
        /// 艾达币
        /// </summary>
        [Description("艾达币")]
        ADA,

        /// <summary>
        /// 比特现金
        /// </summary>
        [Description("比特现金")]
        BCH,

        /// <summary>
        /// BSV
        /// </summary>
        [Description("BSV")]
        BSV,

        /// <summary>
        /// 以太坊经典
        /// </summary>
        [Description("以太坊经典")]
        ETC
    }

    /// <summary>
    /// 标价货币
    /// </summary>
    public enum QuotedCurrency
    {
        /// <summary>
        /// 比特币
        /// </summary>
        [Description("比特币")]
        BTC,

        /// <summary>
        /// 以太坊
        /// </summary>
        [Description("以太坊")]
        ETH,

        /// <summary>
        /// 泰达币
        /// </summary>
        [Description("泰达币")]
        USDT,

        /// <summary>
        /// 美元
        /// </summary>
        [Description("美元")]
        USD
    }

    /// <summary>
    /// 行情来源交易所
    /// </summary>
    public enum QuotationExchange
    {
        Bitfinex,

        Bitstamp,

        Gdax,

        Kraken,

        Binance
    }

    /// <summary>
    /// 合约类型
    /// </summary>
    public enum ContractType
    {
        /// <summary>
        /// 定期
        /// </summary>
        [Description("定期")]
        FixedTerm,

        /// <summary>
        /// 永续
        /// </summary>
        [Description("永续")]
        Perpetual
    }

    /// <summary>
    /// 合约方向
    /// </summary>
    public enum ContractDirect
    {
        /// <summary>
        /// 正向
        /// </summary>
        [Description("正向")]
        Forward = 1,

        /// <summary>
        /// 反向
        /// </summary>
        [Description("反向")]
        Reverse = -1
    }

    /// <summary>
    /// 合约时长单位
    /// </summary>
    public enum ContractPeriodUnit
    {
        [Description("天")]
        Day,

        [Description("周")]
        Week,

        [Description("月")]
        Month
    }
}
