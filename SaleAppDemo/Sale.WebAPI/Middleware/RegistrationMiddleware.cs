using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Collections.Specialized;
using System.Text;
using System.IO;

namespace Sale.WebAPI.Middleware
{
    /// <summary>
    /// 注册中间件，用于记录和处理API请求信息
    /// </summary>
    public class RegistrationMiddleware : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // 记录请求开始时间
            var startTime = DateTime.Now;
            
            // 记录请求信息
            await LogRequestAsync(request);
            
            try
            {
                // 调用下一个处理器
                var response = await base.SendAsync(request, cancellationToken);
                
                // 记录响应信息
                await LogResponseAsync(request, response, startTime);
                
                return response;
            }
            catch (Exception ex)
            {
                // 记录异常信息
                LogException(request, ex, startTime);
                throw;
            }
        }

        /// <summary>
        /// 记录请求信息
        /// </summary>
        /// <param name="request">HTTP请求</param>
        private async Task LogRequestAsync(HttpRequestMessage request)
        {
            var logBuilder = new StringBuilder();
            logBuilder.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] 请求开始");
            logBuilder.AppendLine($"方法: {request.Method}");
            logBuilder.AppendLine($"URL: {request.RequestUri}");
            logBuilder.AppendLine($"客户端IP: {GetClientIpAddress(request)}");
            logBuilder.AppendLine($"用户代理: {request.Headers.UserAgent}");
            
            // 记录请求头
            if (request.Headers != null)
            {
                logBuilder.AppendLine("请求头:");
                foreach (var header in request.Headers)
                {
                    logBuilder.AppendLine($"  {header.Key}: {string.Join(", ", header.Value)}");
                }
            }

            // 记录请求体（仅限POST/PUT请求）
            if (request.Content != null && (request.Method == HttpMethod.Post || request.Method == HttpMethod.Put))
            {
                try
                {
                    var content = await request.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(content))
                    {
                        logBuilder.AppendLine($"请求体: {content}");
                    }
                }
                catch (Exception ex)
                {
                    logBuilder.AppendLine($"读取请求体失败: {ex.Message}");
                }
            }

            // 这里可以将日志写入文件或数据库
            System.Diagnostics.Debug.WriteLine(logBuilder.ToString());
        }

        /// <summary>
        /// 记录响应信息
        /// </summary>
        /// <param name="request">HTTP请求</param>
        /// <param name="response">HTTP响应</param>
        /// <param name="startTime">请求开始时间</param>
        private async Task LogResponseAsync(HttpRequestMessage request, HttpResponseMessage response, DateTime startTime)
        {
            var endTime = DateTime.Now;
            var duration = endTime - startTime;

            var logBuilder = new StringBuilder();
            logBuilder.AppendLine($"[{endTime:yyyy-MM-dd HH:mm:ss}] 请求完成");
            logBuilder.AppendLine($"URL: {request.RequestUri}");
            logBuilder.AppendLine($"状态码: {(int)response.StatusCode} {response.StatusCode}");
            logBuilder.AppendLine($"处理时间: {duration.TotalMilliseconds}ms");

            // 记录响应头
            if (response.Headers != null)
            {
                logBuilder.AppendLine("响应头:");
                foreach (var header in response.Headers)
                {
                    logBuilder.AppendLine($"  {header.Key}: {string.Join(", ", header.Value)}");
                }
            }

            // 记录响应体（可选，注意性能影响）
            if (response.Content != null)
            {
                try
                {
                    var content = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(content) && content.Length < 1000) // 限制长度避免日志过大
                    {
                        logBuilder.AppendLine($"响应体: {content}");
                    }
                }
                catch (Exception ex)
                {
                    logBuilder.AppendLine($"读取响应体失败: {ex.Message}");
                }
            }

            // 这里可以将日志写入文件或数据库
            System.Diagnostics.Debug.WriteLine(logBuilder.ToString());
        }

        /// <summary>
        /// 记录异常信息
        /// </summary>
        /// <param name="request">HTTP请求</param>
        /// <param name="ex">异常</param>
        /// <param name="startTime">请求开始时间</param>
        private void LogException(HttpRequestMessage request, Exception ex, DateTime startTime)
        {
            var endTime = DateTime.Now;
            var duration = endTime - startTime;

            var logBuilder = new StringBuilder();
            logBuilder.AppendLine($"[{endTime:yyyy-MM-dd HH:mm:ss}] 请求异常");
            logBuilder.AppendLine($"URL: {request.RequestUri}");
            logBuilder.AppendLine($"处理时间: {duration.TotalMilliseconds}ms");
            logBuilder.AppendLine($"异常类型: {ex.GetType().Name}");
            logBuilder.AppendLine($"异常消息: {ex.Message}");
            logBuilder.AppendLine($"堆栈跟踪: {ex.StackTrace}");

            // 这里可以将日志写入文件或数据库
            System.Diagnostics.Debug.WriteLine(logBuilder.ToString());
        }

        /// <summary>
        /// 获取客户端IP地址
        /// </summary>
        /// <param name="request">HTTP请求</param>
        /// <returns>客户端IP地址</returns>
        private string GetClientIpAddress(HttpRequestMessage request)
        {
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                var httpContext = request.Properties["MS_HttpContext"] as HttpContextWrapper;
                if (httpContext != null)
                {
                    var clientIp = httpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (string.IsNullOrEmpty(clientIp))
                    {
                        clientIp = httpContext.Request.ServerVariables["REMOTE_ADDR"];
                    }
                    return clientIp;
                }
            }
            return "Unknown";
        }
    }
}