using Sale.Domain;
using System.Collections.Generic;
using System.Data.Entity;
using System.Runtime.Remoting.Contexts;

namespace Sale.DAL
{
    [DbConfigurationType(typeof(MyDbConfiguration))]
    public class SaleDbContext : DbContext
    {
        // 构造函数，指定数据库连接字符串名称为 "SaleDbConnection"
        public SaleDbContext() : base("name=SaleDbConnection") { }

        // 产品表的实体集，用于对 Product 实体进行增删查改操作
        public DbSet<Product> Products { get; set; }
        /*
        SaleDbContext 继承自 DbContext，这是 Entity Framework 的核心类，负责数据库连接和实体追踪。
构造函数 : base("name=SaleDbConnection") 指定了连接字符串，实际内容在 App.config 中配置。
public DbSet<Product> Products { get; set; } 表示数据库中有一个 Products 表，映射到 Product 实体。
你可以通过 SaleDbContext.Products 进行增删改查操作，EF 会自动将这些操作映射为 SQL 语句。
如需自定义实体映射关系，可以重写 OnModelCreating 方法。
        */
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // 假设 Product 实体映射到表 "T_Product"
            modelBuilder.Entity<Product>().ToTable("T_Product");

            // 假设 Product.Name 属性映射到表字段 "ProductName"
            modelBuilder.Entity<Product>()
                .Property(p => p.Name)
                .HasColumnName("ProductName");

            // 假设 Product.Count 属性映射到表字段 "ProductCount"
            modelBuilder.Entity<Product>()
                .Property(p => p.Count)
                .HasColumnName("ProductCount");

            base.OnModelCreating(modelBuilder);
        }
    }
}
