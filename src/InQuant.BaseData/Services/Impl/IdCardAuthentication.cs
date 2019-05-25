using InQuant.BaseData.Models.IdCardAuthentication;
using InQuant.Framework.Exceptions;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace InQuant.BaseData.Services.Impl
{
    public class IdCardAuthentication : IIdCardAuthentication
    {
        private const string host = "https://idenauthen.market.alicloudapi.com";
        private const string path = "/idenAuthentication";
        private const string method = "POST";
        private const string appcode = "c56270cfe28c4adeb21e4ce2979361cf";

        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        public async Task<IdCardInfo> Auth(string idNo, string name)
        {
            string querys = "";
            string bodys = $"idNo={idNo}&name={HttpUtility.UrlEncode(name)}";
            string url = host + path;
            HttpWebRequest httpRequest = null;
            HttpWebResponse httpResponse = null;

            if (0 < querys.Length)
            {
                url = url + "?" + querys;
            }

            if (host.Contains("https://"))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                httpRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
            }
            else
            {
                httpRequest = (HttpWebRequest)WebRequest.Create(url);
            }
            httpRequest.Method = method;
            httpRequest.Headers.Add("Authorization", "APPCODE " + appcode);
            //根据API的要求，定义相对应的Content-Type
            httpRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            if (0 < bodys.Length)
            {
                byte[] data = Encoding.UTF8.GetBytes(bodys);
                using (Stream stream = await httpRequest.GetRequestStreamAsync())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            try
            {
                httpResponse = (HttpWebResponse)(await httpRequest.GetResponseAsync());
            }
            catch (WebException ex)
            {
                httpResponse = (HttpWebResponse)ex.Response;
            }

            using (Stream st = httpResponse.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8")))
                {
                    var content = reader.ReadToEnd();

                    var m = JsonConvert.DeserializeObject<IdCardInfo>(content);
                    if (m.RespCode != "0000")
                    {
                        throw new HopexException(m.RespMessage ?? "身份证实名认证失败");
                    }

                    return m;
                }
            }
        }
    }
}
