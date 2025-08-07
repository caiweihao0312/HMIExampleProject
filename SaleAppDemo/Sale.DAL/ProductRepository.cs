using System.Collections.Generic;
using System.Linq;
using Sale.Domain;

namespace Sale.DAL
{
    // ProductRepository 类实现了 IProductRepository 接口，负责对 Product 实体进行数据库操作
    public class ProductRepository : IProductRepository
    {
        // 创建数据库上下文对象，用于操作数据库
        private readonly SaleDbContext _context = new SaleDbContext();

        // 获取所有产品，返回一个产品集合
        public IEnumerable<Product> GetAll() => _context.Products.ToList();

        // 添加一个新产品到数据库
        public void Add(Product product)
        {
            _context.Products.Add(product); // 添加到上下文
            _context.SaveChanges();         // 保存更改到数据库
        }

        // 更新一个已存在的产品信息
        public void Update(Product product)
        {
            // 标记实体为已修改
            _context.Entry(product).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges(); // 保存更改
        }

        // 根据产品ID删除产品
        public void Delete(int id)
        {
            var product = _context.Products.Find(id); // 查找产品
            if (product != null)
            {
                _context.Products.Remove(product); // 从上下文移除
                _context.SaveChanges();            // 保存更改
            }
        }

        // 根据产品ID获取单个产品
        public Product GetById(int id) => _context.Products.Find(id);

        // 如果数据库不存在则创建数据库
        public void CreateDatabaseIfNotExists()
        {
            _context.Database.CreateIfNotExists();
        }
    }
}