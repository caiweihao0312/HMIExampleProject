using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Sale.BLL;
using Sale.Domain;
using Sale.WebAPI.Filters;

namespace Sale.WebAPI.Controllers
{
    /// <summary>
    /// 产品API控制器，提供产品的CRUD操作接口
    /// </summary>
    //在整个控制器类上应用过滤器，会影响该控制器的所有Action
    //[CustomAuthorizationFilter] // 整个控制器需要身份验证
    //[LoggingActionFilter]       // 整个控制器记录日志
    public class ProductsController : ApiController
    {
        private readonly ProductService _productService;

        /// <summary>
        /// 构造函数，初始化产品服务
        /// </summary>
        public ProductsController()
        {
            _productService = new ProductService();
        }

        /// <summary>
        /// 获取所有产品
        /// GET api/products
        /// </summary>
        /// <returns>所有产品的集合</returns>
        [AllowAnonymous] // 允许匿名访问
        //[LoggingActionFilter] // 记录日志
        [SkipLogging] // 跳过全局日志记录过滤器
        public IEnumerable<Product> Get()
        {
            try
            {
                return _productService.GetAll();
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(Request.CreateErrorResponse(
                    HttpStatusCode.InternalServerError, 
                    $"获取产品列表失败: {ex.Message}"));
            }
        }

        /// <summary>
        /// 根据ID获取单个产品
        /// GET api/products/5
        /// </summary>
        /// <param name="id">产品ID</param>
        /// <returns>指定ID的产品</returns>
        [AllowAnonymous] // 允许匿名访问
        [PerformanceActionFilter] // 性能监控
        //[SkipPerformanceMonitoring] // 跳过性能监控过滤器
        public IHttpActionResult Get(int id)
        {
            try
            {
                var product = _productService.GetById(id);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"获取产品失败: {ex.Message}"));
            }
        }

        /// <summary>
        /// 创建新产品
        /// POST api/products
        /// </summary>
        /// <param name="product">要创建的产品对象</param>
        /// <returns>创建结果</returns>
        [CustomAuthorizationFilter] // 需要身份验证
        [ModelValidationFilter] // 模型验证
        [LoggingActionFilter] // 记录日志
        [PerformanceActionFilter] // 性能监控
        public IHttpActionResult Post([FromBody]Product product)
        {
            try
            {
                if (product == null)
                {
                    return BadRequest("产品数据不能为空");
                }

                if (string.IsNullOrWhiteSpace(product.Name))
                {
                    return BadRequest("产品名称不能为空");
                }

                if (product.Count < 0)
                {
                    return BadRequest("产品数量不能为负数");
                }

                _productService.Add(product);
                return Ok(new { success = true, message = "产品创建成功" });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"创建产品失败: {ex.Message}"));
            }
        }

        /// <summary>
        /// 更新产品信息
        /// PUT api/products/5
        /// </summary>
        /// <param name="id">产品ID</param>
        /// <param name="product">更新的产品信息</param>
        /// <returns>更新结果</returns>
        [CustomAuthorizationFilter] // 需要身份验证
        [ModelValidationFilter] // 模型验证
        public IHttpActionResult Put(int id, [FromBody]Product product)
        {
            try
            {
                if (product == null)
                {
                    return BadRequest("产品数据不能为空");
                }

                if (id != product.Id)
                {
                    return BadRequest("产品ID不匹配");
                }

                var existingProduct = _productService.GetById(id);
                if (existingProduct == null)
                {
                    return NotFound();
                }

                if (string.IsNullOrWhiteSpace(product.Name))
                {
                    return BadRequest("产品名称不能为空");
                }

                if (product.Count < 0)
                {
                    return BadRequest("产品数量不能为负数");
                }

                _productService.Update(product);
                return Ok(new { success = true, message = "产品更新成功" });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"更新产品失败: {ex.Message}"));
            }
        }

        /// <summary>
        /// 删除产品
        /// DELETE api/products/5
        /// </summary>
        /// <param name="id">要删除的产品ID</param>
        /// <returns>删除结果</returns>
        [CustomAuthorizationFilter] // 需要身份验证
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var existingProduct = _productService.GetById(id);
                if (existingProduct == null)
                {
                    return NotFound();
                }

                _productService.Delete(id);
                return Ok(new { success = true, message = "产品删除成功" });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"删除产品失败: {ex.Message}"));
            }
        }

        /// <summary>
        /// 创建数据库（如果不存在）
        /// POST api/products/createdb
        /// </summary>
        /// <returns>创建结果</returns>
        [HttpPost]
        [Route("api/products/createdb")]
        [AllowAnonymous] // 允许匿名访问
        [ExceptionHandlingFilter] // 特殊的异常处理
        public IHttpActionResult CreateDatabase()
        {
            try
            {
                _productService.CreateDatabaseIfNotExists();
                return Ok(new { success = true, message = "数据库创建成功" });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"创建数据库失败: {ex.Message}"));
            }
        }

        /// <summary>
        /// 测试多个过滤器组合
        /// GET api/products/test-filters
        /// </summary>
        [HttpGet]
        [Route("api/products/test-filters")]
        [AllowAnonymous] // 允许匿名访问
        [LoggingActionFilter] // 记录日志
        [PerformanceActionFilter] // 性能监控
        [ModelValidationFilter] // 模型验证
        public IHttpActionResult TestFilters(string name = "测试")
        {
            // 模拟一些处理时间
            System.Threading.Thread.Sleep(500);
            
            return Ok(new { 
                message = "过滤器组合测试",
                parameter = name,
                timestamp = DateTime.Now,
                note = "检查Debug输出查看过滤器执行情况"
            });
        }

        /// <summary>
        /// 测试中间件功能的端点
        /// GET api/products/test-middleware
        /// </summary>
        [HttpGet]
        [Route("api/products/test-middleware")]
        [AllowAnonymous] // 允许匿名访问
        public IHttpActionResult TestMiddleware()
        {
            return Ok(new { 
                message = "中间件测试端点",
                timestamp = DateTime.Now,
                note = "请检查Debug输出窗口查看中间件日志"
            });
        }

        /// <summary>
        /// 测试中间件异常处理
        /// GET api/products/test-exception
        /// </summary>
        [HttpGet]
        [Route("api/products/test-exception")]
        [AllowAnonymous] // 允许匿名访问
        public IHttpActionResult TestException()
        {
            throw new InvalidOperationException("这是一个测试异常，用于验证中间件的异常处理功能");
        }
    }
}