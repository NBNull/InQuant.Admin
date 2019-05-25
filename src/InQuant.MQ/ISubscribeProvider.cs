using InQuant.Framework.Modules;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InQuant.MQ
{
    /// <summary>
    /// 订阅模式provider
    /// </summary>
    public interface ISubscribeProvider : ISingleton
    {
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="consumer"></param>
        void Scribe(IMessageConsumer consumer);

        /// <summary>
        /// 开始监听消息
        /// </summary>
        List<Task> Listen(CancellationToken token);
    }
}
