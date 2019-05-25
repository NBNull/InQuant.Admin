using InQuant.BaseData.Models;
using InQuant.Framework.Exceptions;
using InQuant.Framework.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace InQuant.BaseData.Wallets.Impl
{
    public class WalletInvoker : IWalletInvoker
    {
        private readonly ILogger _logger;
        private readonly BaseDataOptions _baseDataOptions;
        private readonly HttpClient _httpClient;
        private readonly IDictionary<string, string> _basePara;

        public WalletInvoker(WalletHttpClient httpClient, ILogger<WalletInvoker> logger, IOptionsSnapshot<BaseDataOptions> baseDataOptions)
        {
            _logger = logger;
            _httpClient = httpClient.Client;
            _baseDataOptions = baseDataOptions.Value;
            _basePara = new Dictionary<string, string>()
            {
                { "appid", _baseDataOptions.WalletAppId },
                { "appkey", _baseDataOptions.WalletAppKey }
            };
        }

        private IDictionary<string, string> ConcatBasePara(IDictionary<string, string> paras)
        {
            foreach (var kv in _basePara)
            {
                paras.Add(kv);
            }

            return paras;
        }

        public async Task<DataType> Post<DataType>(string url, IDictionary<string, string> paras)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));
            if (paras == null) throw new ArgumentNullException(nameof(paras));

            paras = ConcatBasePara(paras);

            string paras_json = JsonConvert.SerializeObject(paras);
            _logger.LogInformation("post {url} , parameter: {parameter}", new Uri(_httpClient.BaseAddress, url), paras_json);

            string paras_json_cihertext = await SecurityUtil.DESEncrypt(paras_json, _baseDataOptions.WalletDesKey, _baseDataOptions.WalletDesIV);
            var form = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("param", paras_json_cihertext)
            });

            var rps = await _httpClient.PostAsync(url, form);
            rps.EnsureSuccessStatusCode();

            var rps_json = await rps.Content.ReadAsStringAsync(); //返回数据没有加密
           
            _logger.LogInformation("received: {rps_json}", rps_json);

            var rps_m = JsonConvert.DeserializeObject<WalletResponse<DataType>>(rps_json);
            if (rps_m.Status != "1")
            {
                throw new HopexException(rps_m.Message);
            }

            return rps_m.Data;
        }
    }
}
