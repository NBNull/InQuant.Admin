using System.Net.Http;

namespace InQuant.BaseData.Wallets
{
    public class WalletHttpClient
    {
        public WalletHttpClient(HttpClient httpClient)
        {
            Client = httpClient;
        }

        public HttpClient Client { get; set; }
    }
}
