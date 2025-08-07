using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Sale.WebAPI.Middleware;
using Sale.WebAPI.Filters;

namespace Sale.WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            /*
             * 主要功能说明
               我为您创建的注册中间件包含以下功能：
               1.	请求日志记录 - 记录HTTP方法、URL、客户端IP、用户代理等信息
               2.	响应日志记录 - 记录状态码、处理时间、响应头等信息
               3.	异常处理 - 捕获并记录处理过程中的异常
               4.	性能监控 - 计算请求处理时间
               5.	请求体记录 - 对POST/PUT请求记录请求体内容
               使用方式
               中间件已经在WebApiConfig.cs中注册，会自动拦截所有API请求。您可以根据需要：
               •	修改日志输出方式（文件、数据库等）
               •	调整记录的详细程度
               •	添加过滤条件（如排除某些路径）
               •	集成到现有的日志框架中
             */

            // 注册中间件
            config.MessageHandlers.Add(new RegistrationMiddleware());

            /*
             * 过滤器功能说明
               我创建的过滤器包含以下功能：
               1.	LoggingActionFilter - 记录Action的执行过程和参数
               2.	PerformanceActionFilter - 监控Action执行时间，性能分析
               3.	ExceptionHandlingFilter - 全局异常处理，统一错误响应格式
               4.	CustomAuthorizationFilter - 自定义身份验证，支持Bearer Token和API Key
               5.	ModelValidationFilter - 模型验证，自动检查请求参数的有效性
               测试过滤器功能
               您可以通过以下方式测试过滤器：
               1.	匿名访问测试：访问GET /api/products
               2.	身份验证测试：访问POST /api/products（需要在请求头添加Authorization或X-API-Key）
               3.	异常处理测试：访问GET /api/products/test-exception
               4.	性能监控测试：查看响应头中的X-Execution-Time
               这些过滤器将帮助您实现统一的日志记录、异常处理、身份验证和性能监控功能。
             */

            // 注册全局过滤器
            // 当前在WebApiConfig.cs中已经配置了全局过滤器，这些过滤器会自动应用到所有控制器和Action
            // 如何在控制器中应用这些过滤器。有几种不同的应用方式：
            // 1、全局应用；2、控制器级别应用；3、Action级别应用（精细控制）；4、创建专门的示例控制器
            /*
             * 过滤器应用的最佳实践
               1. 优先级顺序
               过滤器的执行顺序为：
               1.	全局过滤器（WebApiConfig中注册的）
               2.	控制器级别过滤器
               3.	Action级别过滤器
               2. 覆盖和继承
               •	[AllowAnonymous] 可以覆盖身份验证要求
               •	Action级别的过滤器会与控制器级别的过滤器叠加
               •	相同类型的过滤器可能会重复执行
               3. 推荐的应用策略
                // 全局应用的过滤器（适用于所有API）
                config.Filters.Add(new ExceptionHandlingFilter());
                config.Filters.Add(new LoggingActionFilter());
                config.Filters.Add(new PerformanceActionFilter());
                
                // 控制器级别（适用于需要特殊处理的控制器）
                [CustomAuthorizationFilter] // 需要身份验证的控制器
                
                // Action级别（适用于特殊需求的方法）
                [AllowAnonymous] // 覆盖身份验证要求
                [ModelValidationFilter] // 特定的模型验证
             */
            config.Filters.Add(new ExceptionHandlingFilter());
            config.Filters.Add(new LoggingActionFilter());
            config.Filters.Add(new PerformanceActionFilter());
            config.Filters.Add(new ModelValidationFilter());
            
            // 如果需要全局身份验证，取消下面的注释
            // config.Filters.Add(new CustomAuthorizationFilter());

            // Web API 配置和服务

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}