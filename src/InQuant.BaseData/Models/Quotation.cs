namespace InQuant.BaseData.Models
{
    /// <summary>
    /// 行情
    /// </summary>
    public class Quotation
    {
        /// <summary>
        /// 币对
        /// </summary>
        public string Item { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }
    }
}
