using System.Threading.Tasks;

namespace InQuant.DingTalk.Services
{
    public interface IDingTalkService
    {
        /// <summary>
        /// 给钉钉指定的webhook发送消息
        /// </summary>
        /// <param name="content"></param>
        /// <param name="isAtAll"></param>
        /// <param name="atMobiles"></param>
        /// <returns></returns>
        Task SendNotify(string webhook, string content, bool isAtAll, params string[] atMobiles);

        /// <summary>
        /// 发送markdown消息
        /// </summary>
        /// <param name="webhook"></param>
        /// <param name="title"></param>
        /// <param name="text"></param>
        /// <param name="isAtAll"></param>
        /// <param name="atMobiles"></param>
        /// <returns></returns>
        Task SendMarkdownNotify(string webhook,
            string title,
            string text,
            bool isAtAll,
            params string[] atMobiles);
    }
}
