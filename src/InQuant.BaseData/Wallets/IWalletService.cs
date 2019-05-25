using InQuant.BaseData.Models.Wallets;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InQuant.BaseData.Wallets
{
    public interface IWalletService
    {
        /// <summary>
        /// 获取钱包地址
        /// </summary>
        /// <param name="asset">币种</param>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        Task<List<WalletAddress>> GetWallets(int userId, params string[] asset);

        /// <summary>
        /// 根据address查所有入金记录
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        Task<List<TXModel>> GetDepositList(string asset, string address);

        /// <summary>
        /// 获取最近几天的入金记录（所有hopex用户的）
        /// </summary>
        /// <param name="daycount"></param>
        /// <returns></returns>
        Task<List<TXModel>> GetAllDepositList(int daycount = 1);

        /// <summary>
        /// 查询出金申请状态（单个或多个）
        /// </summary>
        /// <param name="withdrawIds"></param>
        /// <returns></returns>
        Task<List<WithdrawModel>> GetWithdraws(params int[] withdrawIds);

        /// <summary>
        /// 出金申请
        /// </summary>
        /// <param name="asset">要出金的币种</param>
        /// <param name="address">要出金的地址</param>
        /// <param name="amount">出金金额</param>
        /// <param name="applyid">出金申请的唯一标识（同一个appid+assetcode+applyid多次申请出金，返回同一个withdrawid）</param>
        /// <returns>提现申请id。可根据withdrawid查询提现状态</returns>
        Task<int> Withdraw(string asset, string address, decimal amount, string applyid);
    }
}
