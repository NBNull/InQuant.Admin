using InQuant.Framework.Exceptions;
using InQuant.Framework.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;

namespace InQuant.Admin.Web.Filters
{
    public class ExceptionHandler
    {
        public static JsonResult ExceptionToJson(Exception ex, IStringLocalizer localizer)
        {
            if (ex is HopexException)
            {
                var e = ex as HopexException;
                return new JsonResult(new ApiResponseModel()
                {
                    Ret = -1,
                    ErrCode = e.ErrCode,
                    ErrStr = e.ErrMsg
                });
            }
            //else if (ex is InvokerFailException)
            //{
            //    var e = ex as InvokerFailException;
            //    return new JsonResult(new ApiResponseModel()
            //    {
            //        Ret = -1,
            //        ErrCode = e.ErrCode,
            //        ErrStr = e.Message
            //    });
            //}
            else
            {
                return new JsonResult(new ApiResponseModel()
                {
                    Ret = -1,
                    ErrCode = null,
                    ErrStr = ex.Message ?? localizer["内部错误，请联系InQuant客服"]
                });
            }
        }
    }
}
