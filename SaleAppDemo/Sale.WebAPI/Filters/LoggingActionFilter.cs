using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Sale.WebAPI.Filters
{
    /// <summary>
    /// 日志记录Action过滤器
    /// </summary>
    public class LoggingActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            // 检查是否应该跳过此过滤器
            if (ShouldSkipFilter(actionContext))
            {
                base.OnActionExecuting(actionContext);
                return;
            }

            var controllerName = actionContext.ControllerContext.ControllerDescriptor.ControllerName;
            var actionName = actionContext.ActionDescriptor.ActionName;
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            
            Debug.WriteLine($"[{timestamp}] Action开始执行: {controllerName}.{actionName}");
            
            // 记录请求参数
            if (actionContext.ActionArguments.Count > 0)
            {
                Debug.WriteLine("请求参数:");
                foreach (var arg in actionContext.ActionArguments)
                {
                    Debug.WriteLine($"  {arg.Key}: {arg.Value}");
                }
            }
            
            base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            // 检查是否应该跳过此过滤器
            if (ShouldSkipFilter(actionExecutedContext.ActionContext))
            {
                base.OnActionExecuted(actionExecutedContext);
                return;
            }

            var controllerName = actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName;
            var actionName = actionExecutedContext.ActionContext.ActionDescriptor.ActionName;
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            
            if (actionExecutedContext.Exception != null)
            {
                Debug.WriteLine($"[{timestamp}] Action执行异常: {controllerName}.{actionName} - {actionExecutedContext.Exception.Message}");
            }
            else
            {
                var statusCode = actionExecutedContext.Response?.StatusCode;
                Debug.WriteLine($"[{timestamp}] Action执行完成: {controllerName}.{actionName} - 状态码: {statusCode}");
            }
            
            base.OnActionExecuted(actionExecutedContext);
        }

        private bool ShouldSkipFilter(HttpActionContext actionContext)
        {
            // 检查Action级别的跳过属性
            if (actionContext.ActionDescriptor.GetCustomAttributes<SkipLoggingAttribute>().Any())
            {
                return true;
            }

            // 检查Controller级别的跳过属性
            if (actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<SkipLoggingAttribute>().Any())
            {
                return true;
            }

            return false;
        }
    }
}