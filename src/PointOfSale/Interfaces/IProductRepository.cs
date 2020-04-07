using PointOfSale.Models;

namespace PointOfSale.Interfaces
{
    public interface IProductRepository
    {
        Product GetProduct(string code);

        void AddProduct(Product product);
    }
}