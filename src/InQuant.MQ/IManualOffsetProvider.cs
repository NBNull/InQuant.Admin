using InQuant.Framework.Modules;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InQuant.MQ
{
    /// <summary>
    /// manual manage offset provider
    /// </summary>
    public interface IManualOffsetProvider : ISingleton
    {
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="topics">主题列表</param>
        /// <param name="handle">参数：topic, message</param>
        void Scribe(IMessageConsumer consumer);

        /// <summary>
        /// 开始监听消息
        /// </summary>
        List<Task> Listen(CancellationToken token);
    }
}
