using System.Collections.Generic;

namespace InQuant.BaseData.Wallets
{
    /// <summary>
    /// 钱包请求模型
    /// </summary>
    public class WalletResponse<T>
    {
        /// <summary>
        /// 状态，1成功，-1失败
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 对应失败原因
        /// </summary>
        public string Message { get; set; }

        public T Data { get; set; }
    }

    public class WalletListData<T>
    {
        public List<T> List { get; set; }
    }
}
