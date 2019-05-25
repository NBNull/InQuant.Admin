using InQuant.BaseData.Models.IdCardAuthentication;
using System.Threading.Tasks;

namespace InQuant.BaseData.Services
{
    public interface IIdCardAuthentication
    {
        /// <summary>
        /// 身份证二要素一致性验证
        /// </summary>
        /// <param name="idNo">身份证号码</param>
        /// <param name="name">姓名</param>
        /// <returns></returns>
        Task<IdCardInfo> Auth(string idNo, string name);
    }
}
