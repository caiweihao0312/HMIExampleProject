using System;
using System.Threading;
using System.Web.Http;
using Sale.WebAPI.Filters;

namespace Sale.WebAPI.Controllers
{
    /// <summary>
    /// 过滤器跳过演示控制器
    /// </summary>
    [Route("api/filterskipdemo")]
    public class FilterSkipDemoController : ApiController
    {
        /*
         * 测试跳过功能
           1.	测试标准接口：GET /api/filterskipdemo/standard
           •	会看到完整的日志和性能监控
           2.	测试静默接口：GET /api/filterskipdemo/silent
           •	不会在Debug输出中看到日志记录
           3.	测试快速接口：GET /api/filterskipdemo/fast
           •	响应头中不会包含X-Execution-Time
           4.	测试自由接口：POST /api/filterskipdemo/free
           •	可以发送任意格式的数据，不会进行模型验证
           5.	测试原生异常：GET /api/filterskipdemo/rawexception
           •	会抛出原生异常，不会被格式化为统一的错误响应
         */

        /// <summary>
        /// 标准接口 - 使用所有全局过滤器
        /// GET api/filterskipdemo/standard
        /// </summary>
        [HttpGet]
        [Route("api/filterskipdemo/standard")]
        [AllowAnonymous]
        public IHttpActionResult Standard()
        {
            Thread.Sleep(100); // 模拟处理时间
            return Ok(new { 
                message = "标准接口 - 使用所有全局过滤器",
                timestamp = DateTime.Now
            });
        }

        /// <summary>
        /// 静默接口 - 跳过日志记录
        /// GET api/filterskipdemo/silent
        /// </summary>
        [HttpGet]
        [Route("api/filterskipdemo/silent")]
        [AllowAnonymous]
        [SkipLogging] // 不记录日志
        public IHttpActionResult Silent()
        {
            return Ok(new { 
                message = "静默接口 - 跳过日志记录",
                timestamp = DateTime.Now
            });
        }

        /// <summary>
        /// 快速接口 - 跳过性能监控
        /// GET api/filterskipdemo/fast
        /// </summary>
        [HttpGet]
        [Route("api/filterskipdemo/fast")]
        [AllowAnonymous]
        [SkipPerformanceMonitoring] // 不监控性能
        public IHttpActionResult Fast()
        {
            return Ok(new { 
                message = "快速接口 - 跳过性能监控",
                timestamp = DateTime.Now,
                note = "响应头中不会包含X-Execution-Time"
            });
        }

        /// <summary>
        /// 自由接口 - 跳过模型验证
        /// POST api/filterskipdemo/free
        /// </summary>
        [HttpPost]
        [Route("api/filterskipdemo/free")]
        [AllowAnonymous]
        [SkipModelValidation] // 不验证模型
        public IHttpActionResult Free([FromBody]object data)
        {
            return Ok(new { 
                message = "自由接口 - 跳过模型验证",
                receivedData = data,
                timestamp = DateTime.Now
            });
        }

        /// <summary>
        /// 原生异常接口 - 跳过异常处理
        /// GET api/filterskipdemo/rawexception
        /// </summary>
        [HttpGet]
        [Route("api/filterskipdemo/rawexception")]
        [AllowAnonymous]
        [SkipExceptionHandling] // 不处理异常
        public IHttpActionResult RawException()
        {
            throw new InvalidOperationException("这是原生异常，不会被格式化");
        }

        /// <summary>
        /// 极简接口 - 跳过所有过滤器
        /// GET api/filterskipdemo/minimal
        /// </summary>
        [HttpGet]
        [Route("api/filterskipdemo/minimal")]
        [AllowAnonymous]
        [SkipLogging]
        [SkipPerformanceMonitoring]
        [SkipModelValidation]
        [SkipExceptionHandling]
        public IHttpActionResult Minimal()
        {
            return Ok(new { 
                message = "极简接口 - 跳过所有全局过滤器",
                timestamp = DateTime.Now,
                note = "此接口完全跳过了全局过滤器的处理"
            });
        }
    }
}