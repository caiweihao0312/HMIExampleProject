using System.Collections.Generic;
using Sale.Domain;
using Sale.DAL;

namespace Sale.BLL
{
    public class ProductService
    {
        private readonly IProductRepository _repo = new ProductRepository();

        public IEnumerable<Product> GetAll()
        {
            return _repo.GetAll();
        }
        public void Add(Product product)
        {
            _repo.Add(product);
        }

        public void Update(Product product)
        {
             _repo.Update(product);
        }
    
        public void Delete(int id)
        {
            _repo.Delete(id);
        }

        public Product GetById(int id)
        {
            return _repo.GetById(id);
        }
        
        public void CreateDatabaseIfNotExists()
        {
            _repo.CreateDatabaseIfNotExists();
        }
    }
}