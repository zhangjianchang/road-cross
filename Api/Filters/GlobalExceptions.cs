using Api.Entity;
using Api.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Filters
{
    public class GlobalExceptions : IExceptionFilter

    {

        public GlobalExceptions()
        {

        }

        public void OnException(ExceptionContext context)
        {
            string path = context.HttpContext.Request.Path;
            string url = context.HttpContext.Request.Host + path + context.HttpContext.Request.QueryString;
            string method = context.HttpContext.Request.Method;

            if (context.Exception is MsgException)
            {
                // 返回ERROR响应码
                Logger.Default.Error("已捕获异常", context.Exception.Message, path, method, "");
                context.Result = new JsonResult(MyResult.Error(context.Exception.Message));
                context.ExceptionHandled = true;
                context.HttpContext.Response.Clear();
                context.HttpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
                context.HttpContext.Response.StatusCode = 200;
            }
            else
            {
                Logger.Default.Error(context.Exception, path, method);
            }
        }

    }
}
