using InQuant.Framework.Modules;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace InQuant.MQ
{
    public interface IMessageConsumer : ISingleton
    {
        /// <summary>
        /// 消费类型
        /// </summary>
        ConsumerType Type { get; }

        /// <summary>
        /// 关注的主题
        /// </summary>
        string Topic { get; }

        /// <summary>
        /// message key是否忽略
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool IgnoreKey(string key);

        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="key">message key</param>
        /// <param name="value">message value</param>
        /// <param name="scope"></param>
        /// <returns></returns>
        Task Handler(IServiceScope scope, string key, string value);
    }

    public enum ConsumerType
    {
        /// <summary>
        /// 订阅模式（多实例部署只会有一个实例消费）
        /// </summary>
        Subscribe,

        /// <summary>
        /// 手动管理偏移量的模式（多实例能够同时消费）
        /// </summary>
        ManaualOffset
    }
}
