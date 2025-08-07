using System;

namespace Sale.WebAPI.Filters
{
    /// <summary>
    /// 跳过日志记录过滤器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SkipLoggingAttribute : Attribute
    {
    }

    /// <summary>
    /// 跳过性能监控过滤器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SkipPerformanceMonitoringAttribute : Attribute
    {
    }

    /// <summary>
    /// 跳过模型验证过滤器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SkipModelValidationAttribute : Attribute
    {
    }

    /// <summary>
    /// 跳过异常处理过滤器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SkipExceptionHandlingAttribute : Attribute
    {
    }
}