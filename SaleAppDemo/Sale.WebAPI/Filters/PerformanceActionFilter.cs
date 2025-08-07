using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Sale.WebAPI.Filters
{
    /// <summary>
    /// 性能监控Action过滤器
    /// </summary>
    public class PerformanceActionFilter : ActionFilterAttribute
    {
        private const string StopwatchKey = "PerformanceFilter.Stopwatch";

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            // 检查是否应该跳过此过滤器
            if (ShouldSkipFilter(actionContext))
            {
                base.OnActionExecuting(actionContext);
                return;
            }

            var stopwatch = Stopwatch.StartNew();
            actionContext.Request.Properties[StopwatchKey] = stopwatch;
            
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

            if (actionExecutedContext.Request.Properties.TryGetValue(StopwatchKey, out object value) 
                && value is Stopwatch stopwatch)
            {
                stopwatch.Stop();
                
                var controllerName = actionExecutedContext.ActionContext.ControllerContext.ControllerDescriptor.ControllerName;
                var actionName = actionExecutedContext.ActionContext.ActionDescriptor.ActionName;
                var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                
                Debug.WriteLine($"[性能监控] {controllerName}.{actionName} 执行时间: {elapsedMilliseconds}ms");
                
                // 如果执行时间超过1秒，记录警告
                if (elapsedMilliseconds > 1000)
                {
                    Debug.WriteLine($"[性能警告] {controllerName}.{actionName} 执行时间过长: {elapsedMilliseconds}ms");
                }
                
                // 将执行时间添加到响应头
                if (actionExecutedContext.Response != null)
                {
                    actionExecutedContext.Response.Headers.Add("X-Execution-Time", $"{elapsedMilliseconds}ms");
                }
            }
            
            base.OnActionExecuted(actionExecutedContext);
        }

        private bool ShouldSkipFilter(HttpActionContext actionContext)
        {
            // 检查Action级别的跳过属性
            if (actionContext.ActionDescriptor.GetCustomAttributes<SkipPerformanceMonitoringAttribute>().Any())
            {
                return true;
            }

            // 检查Controller级别的跳过属性
            if (actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<SkipPerformanceMonitoringAttribute>().Any())
            {
                return true;
            }

            return false;
        }
    }
}