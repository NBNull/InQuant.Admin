using System.Threading.Tasks;

namespace InQuant.BaseData.Services
{
    public interface INoGenerator
    {
        /// <summary>
        /// 生成下一个No  [category][日期][0000-9999]
        /// eg: xxx201808270001
        /// </summary>
        /// <param name="category"></param>
        /// <param name="incrementBit">自增序列位数（默认4位）</param>
        /// <returns></returns>
        Task<string> NextNo(string category, byte incrementBit = 4);
    }
}
