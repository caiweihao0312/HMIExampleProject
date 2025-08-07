using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;

public class MyDbConfiguration : DbConfiguration
{
    public MyDbConfiguration()
    {
        // 这里可以设置EF的Provider、策略等
        SetDefaultConnectionFactory(new System.Data.Entity.Infrastructure.LocalDbConnectionFactory("mssqllocaldb"));


        // 示例：全局注册 SQL 拦截器、日志拦截器或性能计数器
        //DbInterception.Add(new MyCommandInterceptor());
    }
}