using InQuant.BaseData.Models;
using InQuant.BaseData.Models.Wallets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InQuant.BaseData.Wallets.Impl
{
    public class WalletService : IWalletService
    {
        private readonly ILogger _logger;
        private readonly BaseDataOptions _baseDataOptions;
        private readonly IWalletInvoker _walletInvoker;

        public WalletService(ILogger<WalletService> logger, IOptionsSnapshot<BaseDataOptions> options, IWalletInvoker walletInvoker)
        {
            _logger = logger;
            _baseDataOptions = options.Value;
            _walletInvoker = walletInvoker;
        }

        public async Task<List<TXModel>> GetDepositList(string asset, string address)
        {
            var paras = new Dictionary<string, string>()
            {
                { "assetcode", asset ?? throw new ArgumentNullException(nameof(asset)) },
                { "address", address ?? throw new ArgumentNullException(nameof(address)) }
            };

            var data = await _walletInvoker.Post<WalletListData<TXModel>>(_baseDataOptions.GetTXList, paras);

            return data.List;
        }

        public async Task<List<TXModel>> GetAllDepositList(int daycount = 1)
        {
            var paras = new Dictionary<string, string>()
            {
                { "date", DateTime.Today.AddDays(-daycount).ToString("yyyy-MM-dd") },
                { "daycount", (daycount+1).ToString() }
            };

            var data = await _walletInvoker.Post<WalletListData<TXModel>>(_baseDataOptions.GetAppidTXlist, paras);

            return data.List;
        }

        public async Task<List<WalletAddress>> GetWallets(int userId, params string[] asset)
        {
            if (userId <= 0) throw new ArgumentException("userId must great zero");
            if (asset.Length == 0) throw new ArgumentException("asset must not empty");

            var paras = new Dictionary<string, string>()
            {
                { "cwid", userId.ToString() },
                { "assetcodes", string.Join(",", asset) }
            };

            var data = await _walletInvoker.Post<WalletListData<WalletAddress>>(_baseDataOptions.Getnewaddress, paras);

            return data.List;
        }

        public async Task<List<WithdrawModel>> GetWithdraws(params int[] withdrawIds)
        {
            if (withdrawIds.Length == 0) throw new ArgumentException("withdrawIds must not empty");

            var paras = new Dictionary<string, string>()
            {
                { "withdrawids", string.Join(",",withdrawIds.Select(x=>x.ToString())) }
            };

            var data = await _walletInvoker.Post<WalletListData<WithdrawModel>>(_baseDataOptions.GetWithdrawList, paras);

            return data.List;
        }

        public async Task<int> Withdraw(string asset, string address, decimal amount, string applyid)
        {
            if (string.IsNullOrWhiteSpace(asset)) throw new ArgumentNullException(nameof(asset));
            if (string.IsNullOrWhiteSpace(address)) throw new ArgumentNullException(nameof(address));
            if (string.IsNullOrWhiteSpace(applyid)) throw new ArgumentNullException(nameof(applyid));
            if (amount <= 0) throw new ArgumentException("amount必须大于0");

            var paras = new Dictionary<string, string>()
            {
               { "assetcode", asset },
               { "address", address },
               { "amount", amount.ToString() },
               { "applyid",  applyid}
            };

            var data = await _walletInvoker.Post<dynamic>(_baseDataOptions.Withdraw, paras);

            string idstr = (string)data.withdrawid;
            if (string.IsNullOrWhiteSpace(idstr))
            {
                _logger.LogError("链上出金，钱包返回的withdrawid为空");
                throw new Exception("钱包返回的提现ID为空，请稍后再试");
            }

            return Convert.ToInt32(idstr);
        }
    }
}
