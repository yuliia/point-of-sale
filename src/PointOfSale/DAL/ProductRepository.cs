using System.Collections.Generic;
using PointOfSale.Interfaces;
using PointOfSale.Models;

namespace PointOfSale.DAL
{
    public class ProductRepository : IProductRepository
    {
        private readonly Dictionary<string, Product> _products = new Dictionary<string, Product>();
        public Product GetProduct(string code)
        {
            return _products.TryGetValue(code, out var product) ? product : null;
        }

        public void AddProduct(Product product)
        {
            _products[product.Code] = product;
        }
    }
}