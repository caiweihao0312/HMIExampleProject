using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Sale.WebAPI.Filters
{
    /// <summary>
    /// 全局异常处理过滤器
    /// </summary>
    public class ExceptionHandlingFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            // 检查是否应该跳过此过滤器
            if (ShouldSkipFilter(actionExecutedContext.ActionContext))
            {
                return; // 不处理异常，让它继续向上抛出
            }

            var exception = actionExecutedContext.Exception;
            var request = actionExecutedContext.Request;
            var controllerName = actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName;
            var actionName = actionExecutedContext.ActionContext.ActionDescriptor.ActionName;
            
            // 记录异常信息
            LogException(exception, controllerName, actionName, request);
            
            // 根据异常类型返回不同的HTTP状态码和错误信息
            HttpResponseMessage response = CreateErrorResponse(exception, request);
            
            actionExecutedContext.Response = response;
        }

        private bool ShouldSkipFilter(HttpActionContext actionContext)
        {
            // 检查Action级别的跳过属性
            if (actionContext.ActionDescriptor.GetCustomAttributes<SkipExceptionHandlingAttribute>().Any())
            {
                return true;
            }

            // 检查Controller级别的跳过属性
            if (actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<SkipExceptionHandlingAttribute>().Any())
            {
                return true;
            }

            return false;
        }

        private void LogException(Exception exception, string controllerName, string actionName, HttpRequestMessage request)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var url = request.RequestUri?.ToString();
            var method = request.Method?.ToString();
            
            System.Diagnostics.Debug.WriteLine($"[{timestamp}] 异常发生:");
            System.Diagnostics.Debug.WriteLine($"  控制器: {controllerName}");
            System.Diagnostics.Debug.WriteLine($"  动作: {actionName}");
            System.Diagnostics.Debug.WriteLine($"  请求: {method} {url}");
            System.Diagnostics.Debug.WriteLine($"  异常类型: {exception.GetType().Name}");
            System.Diagnostics.Debug.WriteLine($"  异常消息: {exception.Message}");
            System.Diagnostics.Debug.WriteLine($"  堆栈跟踪: {exception.StackTrace}");
        }

        private HttpResponseMessage CreateErrorResponse(Exception exception, HttpRequestMessage request)
        {
            HttpStatusCode statusCode;
            string message;
            
            // 根据异常类型确定状态码和消息
            switch (exception)
            {
                case ArgumentNullException _:
                case ArgumentException _:
                    statusCode = HttpStatusCode.BadRequest;
                    message = "请求参数无效";
                    break;
                
                case UnauthorizedAccessException _:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = "未授权访问";
                    break;
                
                case NotImplementedException _:
                    statusCode = HttpStatusCode.NotImplemented;
                    message = "功能尚未实现";
                    break;
                
                case TimeoutException _:
                    statusCode = HttpStatusCode.RequestTimeout;
                    message = "请求超时";
                    break;
                
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    message = "服务器内部错误";
                    break;
            }
            
            var errorResponse = new
            {
                error = true,
                message = message,
                timestamp = DateTime.Now,
                path = request.RequestUri?.PathAndQuery
            };
            
            return request.CreateResponse(statusCode, errorResponse);
        }
    }
}