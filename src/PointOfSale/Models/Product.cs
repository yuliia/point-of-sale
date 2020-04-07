using System;

namespace PointOfSale.Models
{
    public class Product
    {
        public Product(string code, decimal price)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentNullException(nameof(code));
            }
            
            if (price <= 0)
            {
                throw new ArgumentException($"{nameof(price)} should be positive number.");
            }
            
            Code = code;
            Price = price;
        }

        public string Code { get; }

        public decimal Price { get; }
    }
}