using System.Collections.Generic;
using Sale.Domain;

namespace Sale.DAL
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetAll();
        void Add(Product product);
        void Update(Product product);
        void Delete(int id);
        Product GetById(int id);
        void CreateDatabaseIfNotExists(); // ÐÂÔö
    }
}