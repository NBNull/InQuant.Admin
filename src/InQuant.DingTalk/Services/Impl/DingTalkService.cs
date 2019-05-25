using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InQuant.DingTalk.Services.Impl
{
    public class DingTalkService : IDingTalkService
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public DingTalkService(IHttpClientFactory httpClientFactory, ILogger<DingTalkService> logger)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task SendNotify(string webhook, string content, bool isAtAll, params string[] atMobiles)
        {
            try
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    var m = new
                    {
                        msgtype = "text",
                        text = new
                        {
                            content = string.Format("InQuant项目:{0}", content)
                        },
                        at = new
                        {
                            atMobiles = atMobiles,
                            isAtAll = isAtAll
                        }
                    };


                    var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, webhook)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(m), Encoding.UTF8, "application/json")
                    });

                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                //发送钉钉消息出错不能影响业务代码，所以吃掉异常
                _logger.LogError(ex, "发送钉钉消息失败，webhook: {hook}，content: {content}", webhook, content);
            }
        }

        public async Task SendMarkdownNotify(string webhook, 
            string title, 
            string text, 
            bool isAtAll, 
            params string[] atMobiles)
        {
            try
            {
                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    var m = new
                    {
                        msgtype = "markdown",
                        markdown = new
                        {
                            title = title,
                            text = text
                        },
                        at = new
                        {
                            atMobiles = atMobiles,
                            isAtAll = isAtAll
                        }
                    };

                    var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, webhook)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(m), Encoding.UTF8, "application/json")
                    });

                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                //发送钉钉消息出错不能影响业务代码，所以吃掉异常
                _logger.LogError(ex, "发送钉钉Markdown消息失败，webhook: {hook}，content: {content}", webhook, text);
            }
        }
    }
}
