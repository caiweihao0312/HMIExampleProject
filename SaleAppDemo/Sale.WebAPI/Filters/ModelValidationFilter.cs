using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Sale.WebAPI.Filters
{
    /// <summary>
    /// 模型验证过滤器
    /// </summary>
    public class ModelValidationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            // 检查是否应该跳过此过滤器
            if (ShouldSkipFilter(actionContext))
            {
                base.OnActionExecuting(actionContext);
                return;
            }

            if (!actionContext.ModelState.IsValid)
            {
                var errors = actionContext.ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new
                    {
                        Field = x.Key,
                        Errors = x.Value.Errors.Select(e => e.ErrorMessage)
                    })
                    .ToArray();

                var response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    error = true,
                    message = "模型验证失败",
                    details = errors,
                    timestamp = System.DateTime.Now
                });

                actionContext.Response = response;
            }

            base.OnActionExecuting(actionContext);
        }

        private bool ShouldSkipFilter(HttpActionContext actionContext)
        {
            // 检查Action级别的跳过属性
            if (actionContext.ActionDescriptor.GetCustomAttributes<SkipModelValidationAttribute>().Any())
            {
                return true;
            }

            // 检查Controller级别的跳过属性
            if (actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<SkipModelValidationAttribute>().Any())
            {
                return true;
            }

            return false;
        }
    }
}