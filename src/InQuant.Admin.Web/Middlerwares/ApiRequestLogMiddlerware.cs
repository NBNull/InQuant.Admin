using InQuant.Authorization.Token;
using InQuant.Framework.Extensions;
using InQuant.Framework.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InQuant.Admin.Web.Middlerwares
{
    public class ApiRequestLogMiddlerware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        public ApiRequestLogMiddlerware(RequestDelegate next, ILogger<ApiRequestLogMiddlerware> logger)
        {
            _logger = logger;
            _next = next;
        }

        /// <summary>
        /// 时间、用户IP、userID、packType、url、请求json、应答json。注意写日志的时候，json里面如果有密码，那么要remove密码之后再写
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            context.Items["CorrelationId"] = Guid.NewGuid().ToString();

            if (!context.Request.Path.HasValue || !context.Request.Path.Value.Contains("api/", StringComparison.InvariantCultureIgnoreCase))
            {
                await _next(context);
                return;
            }

            var request = await FormatRequest(context.Request);
            var ip = IpUtil.GetClientIp(context);
            var userId = context.User?.GetId();
            var packType = context.Request.GetPackType();

            var originalBodyStream = context.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                Stopwatch watch = Stopwatch.StartNew();

                await _next(context);

                var response = await FormatResponse(context.Response);

                await responseBody.CopyToAsync(originalBodyStream);

                watch.Stop();

                Log(ip, userId, packType.ToString(), request, response, Math.Round(watch.Elapsed.Ticks / 10000.0, 2, MidpointRounding.AwayFromZero));
            }
        }

        private void Log(string ip, int? userId, string packType, string request, string response, double costTimes)
        {
            _logger.LogInformation("{IP} {UserId} {PackType} {Request} {Response} {Costs}ms",
                ip, userId, packType, request, response, costTimes);
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            if (request.ContentType != null && request.ContentType.Contains("multipart/form-data; boundary="))
            {
                //文件上传，不读取body
                return $"{request.Path} {request.QueryString} {request.ContentType}";
            }
            else
            {
                request.EnableRewind();

                var buffer = new byte[Convert.ToInt32(request.ContentLength)];

                await request.Body.ReadAsync(buffer, 0, buffer.Length);

                var bodyAsText = Encoding.UTF8.GetString(buffer);

                request.Body.Position = 0;

                return $"{request.Path} {request.QueryString} {DelPasswordInfo(bodyAsText)}";
            }
        }

        private string DelPasswordInfo(string s)
        {
            return Regex.Replace(s, @"""password""\s*\:\s*"".*?""", @"""password"":""*""");
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            if (response.ContentType != null && (
                response.ContentType.Contains("application/json", StringComparison.InvariantCultureIgnoreCase) ||
                response.ContentType.Contains("text/", StringComparison.InvariantCultureIgnoreCase)))
            {
                response.Body.Seek(0, SeekOrigin.Begin);

                string text = await new StreamReader(response.Body).ReadToEndAsync();

                response.Body.Seek(0, SeekOrigin.Begin);

                return $"{response.StatusCode}: {text}";
            }
            else
            {
                response.Body.Seek(0, SeekOrigin.Begin);
                return $"{response.StatusCode}";
            }
        }
    }
}
