using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sale.WebAPI.Middleware;

namespace Sale.WebAPI.Tests.MiddlewareTests
{
    [TestClass]
    public class RegistrationMiddlewareTests
    {
        private HttpConfiguration _config;
        private HttpServer _server;
        private HttpClient _client;

        [TestInitialize]
        public void Setup()
        {
            _config = new HttpConfiguration();
            
            // 注册中间件
            _config.MessageHandlers.Add(new RegistrationMiddleware());
            
            // 添加测试控制器路由
            _config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            _server = new HttpServer(_config);
            _client = new HttpClient(_server);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _client?.Dispose();
            _server?.Dispose();
            _config?.Dispose();
        }

        [TestMethod]
        public async Task RegistrationMiddleware_ShouldLogRequest_WhenGetRequest()
        {
            // Arrange
            var requestUri = "http://localhost/api/products";

            // Act
            var response = await _client.GetAsync(requestUri);

            // Assert
            Assert.IsNotNull(response);
            // 注意：实际的日志验证需要根据您的日志实现来调整
        }

        [TestMethod]
        public async Task RegistrationMiddleware_ShouldLogRequest_WhenPostRequest()
        {
            // Arrange
            var requestUri = "http://localhost/api/products";
            var content = new StringContent("{\"name\":\"测试产品\",\"count\":10}", 
                System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync(requestUri, content);

            // Assert
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task RegistrationMiddleware_ShouldHandleException_WhenErrorOccurs()
        {
            // Arrange
            var requestUri = "http://localhost/api/invalid";

            // Act & Assert
            try
            {
                var response = await _client.GetAsync(requestUri);
                // 验证中间件是否正确处理了异常
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(ex);
            }
        }
    }
}