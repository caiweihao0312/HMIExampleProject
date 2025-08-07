using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sale.WebAPI.Middleware
{
    /// <summary>
    /// 简单的日志记录中间件
    /// </summary>
    public class SimpleLoggingMiddleware : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var startTime = DateTime.Now;
            
            // 记录请求
            Console.WriteLine($"[{startTime:HH:mm:ss}] {request.Method} {request.RequestUri}");
            
            try
            {
                var response = await base.SendAsync(request, cancellationToken);
                
                var endTime = DateTime.Now;
                var duration = endTime - startTime;
                
                // 记录响应
                Console.WriteLine($"[{endTime:HH:mm:ss}] {request.Method} {request.RequestUri} - {(int)response.StatusCode} ({duration.TotalMilliseconds}ms)");
                
                return response;
            }
            catch (Exception ex)
            {
                var endTime = DateTime.Now;
                var duration = endTime - startTime;
                
                // 记录异常
                Console.WriteLine($"[{endTime:HH:mm:ss}] {request.Method} {request.RequestUri} - ERROR: {ex.Message} ({duration.TotalMilliseconds}ms)");
                
                throw;
            }
        }
    }
}