using System.Collections.Generic;

namespace InQuant.DingTalk.ViewModels
{
    public class SendDingTalkNotifyViewModel
    {
        public string Webhook { get; set; }

        public string Content { get; set; }

        public bool isAtAll { get; set; }

        public List<string> AtMobiles { get; set; }
    }
}
