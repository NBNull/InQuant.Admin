using InQuant.Framework.Data.Core.Sessions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace InQuant.Admin.Web.Filters
{
    public class InQuantExceptionFilter : ExceptionFilterAttribute
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogger<InQuantExceptionFilter> _logger;
        private readonly IStringLocalizer<InQuantExceptionFilter> _localizer;
        private readonly IDapperSessionContext _dapperSessionContext;

        public InQuantExceptionFilter(IHostingEnvironment hostingEnvironment,
            IStringLocalizer<InQuantExceptionFilter> localizer,
            IDapperSessionContext dapperSessionContext,
            ILogger<InQuantExceptionFilter> logger)
        {
            _logger = logger;
            _localizer = localizer;
            _hostingEnvironment = hostingEnvironment;
            _dapperSessionContext = dapperSessionContext;
        }

        public override void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "出现错误.{ERRMSG}", context.Exception.Message);

            _dapperSessionContext.Cancel();

            context.ExceptionHandled = true;

            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                var controllerAttributes = controllerActionDescriptor.ControllerTypeInfo.GetCustomAttributes(false);
                if (controllerAttributes.Any(x => x is ApiControllerAttribute))
                {
                    var jsonResult = ExceptionHandler.ExceptionToJson(context.Exception, _localizer);

                    context.Result = jsonResult;
                }
                else
                {
                    context.Result = new RedirectToActionResult("ErrorCode", "Error", new { code = 500 });
                }
            }
        }
    }
}
