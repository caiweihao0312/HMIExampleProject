using System;
using System.Net;
using System.Web.Http;

namespace Sale.WebAPI.Controllers
{
    /// <summary>
    /// 测试控制器，用于验证中间件功能
    /// </summary>
    [Route("api/test")]
    public class TestController : ApiController
    {
        /// <summary>
        /// 正常响应测试
        /// GET api/test
        /// </summary>
        [HttpGet]
        public IHttpActionResult Get()
        {
            return Ok(new { 
                message = "测试成功", 
                timestamp = DateTime.Now,
                middleware = "RegistrationMiddleware正在工作"
            });
        }

        /// <summary>
        /// 延时响应测试（测试性能监控）
        /// GET api/test/delay
        /// </summary>
        [HttpGet]
        [Route("api/test/delay")]
        public IHttpActionResult GetWithDelay()
        {
            // 模拟处理延时
            System.Threading.Thread.Sleep(2000);
            return Ok(new { 
                message = "延时测试完成", 
                delay = "2秒"
            });
        }

        /// <summary>
        /// POST请求测试（测试请求体记录）
        /// POST api/test
        /// </summary>
        [HttpPost]
        public IHttpActionResult Post([FromBody]object data)
        {
            return Ok(new { 
                message = "POST测试成功", 
                receivedData = data
            });
        }

        /// <summary>
        /// 错误响应测试
        /// GET api/test/error
        /// </summary>
        [HttpGet]
        [Route("api/test/error")]
        public IHttpActionResult GetError()
        {
            return BadRequest("这是一个测试错误");
        }

        /// <summary>
        /// 异常测试
        /// GET api/test/exception
        /// </summary>
        [HttpGet]
        [Route("api/test/exception")]
        public IHttpActionResult GetException()
        {
            throw new InvalidOperationException("这是一个测试异常");
        }

        /// <summary>
        /// 大数据响应测试
        /// GET api/test/bigdata
        /// </summary>
        [HttpGet]
        [Route("api/test/bigdata")]
        public IHttpActionResult GetBigData()
        {
            var largeData = new string('A', 5000); // 5KB的数据
            return Ok(new { 
                message = "大数据测试", 
                data = largeData,
                size = "5KB"
            });
        }
    }
}