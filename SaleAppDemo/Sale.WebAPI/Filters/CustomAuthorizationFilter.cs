using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Sale.WebAPI.Filters
{
    /// <summary>
    /// 自定义身份验证过滤器
    /// </summary>
    public class CustomAuthorizationFilter : Attribute, IAuthorizationFilter
    {
        public bool AllowMultiple => false;

        public async Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(
            HttpActionContext actionContext, 
            CancellationToken cancellationToken, 
            Func<Task<HttpResponseMessage>> continuation)
        {
            // 检查是否有AllowAnonymous属性
            if (HasAllowAnonymousAttribute(actionContext))
            {
                return await continuation();
            }

            // 执行身份验证
            if (!IsAuthorized(actionContext))
            {
                return CreateUnauthorizedResponse(actionContext.Request);
            }

            // 继续执行
            return await continuation();
        }

        private bool HasAllowAnonymousAttribute(HttpActionContext actionContext)
        {
            // 检查Action级别的AllowAnonymous
            if (actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any())
            {
                return true;
            }

            // 检查Controller级别的AllowAnonymous
            if (actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any())
            {
                return true;
            }

            return false;
        }

        private bool IsAuthorized(HttpActionContext actionContext)
        {
            var request = actionContext.Request;
            
            // 检查Authorization头
            if (request.Headers.Authorization != null)
            {
                var scheme = request.Headers.Authorization.Scheme;
                var token = request.Headers.Authorization.Parameter;
                
                // 简单的Bearer Token验证示例
                if (scheme == "Bearer" && IsValidToken(token))
                {
                    return true;
                }
            }
            
            // 检查API Key（从查询参数或头部）
            var apiKey = GetApiKey(request);
            if (!string.IsNullOrEmpty(apiKey) && IsValidApiKey(apiKey))
            {
                return true;
            }
            
            return false;
        }

        private string GetApiKey(HttpRequestMessage request)
        {
            // 从查询参数获取API Key
            var queryParams = request.GetQueryNameValuePairs();
            var apiKeyParam = queryParams.FirstOrDefault(q => q.Key.Equals("apikey", StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(apiKeyParam.Value))
            {
                return apiKeyParam.Value;
            }
            
            // 从头部获取API Key
            if (request.Headers.Contains("X-API-Key"))
            {
                return request.Headers.GetValues("X-API-Key").FirstOrDefault();
            }
            
            return null;
        }

        private bool IsValidToken(string token)
        {
            // 这里应该实现真正的Token验证逻辑
            // 例如：JWT验证、数据库查询等
            
            // 示例：简单的静态Token验证
            var validTokens = new[] { "demo-token-123", "test-token-456" };
            return validTokens.Contains(token);
        }

        private bool IsValidApiKey(string apiKey)
        {
            // 这里应该实现真正的API Key验证逻辑
            // 例如：数据库查询、缓存查询等
            
            // 示例：简单的静态API Key验证
            var validApiKeys = new[] { "demo-api-key-123", "test-api-key-456" };
            return validApiKeys.Contains(apiKey);
        }

        private HttpResponseMessage CreateUnauthorizedResponse(HttpRequestMessage request)
        {
            var response = request.CreateResponse(HttpStatusCode.Unauthorized, new
            {
                error = true,
                message = "未授权访问，请提供有效的API Key或Token",
                timestamp = DateTime.Now
            });
            
            response.Headers.Add("WWW-Authenticate", "Bearer");
            return response;
        }
    }
}