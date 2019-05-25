using System.Collections.Generic;
using System.Threading.Tasks;

namespace InQuant.BaseData.Wallets
{
    public interface IWalletInvoker
    {
        Task<DataType> Post<DataType>(string url, IDictionary<string, string> paras);
    }
}
