using System;

namespace PointOfSale.Models
{
    public class CheckItem
    {
        public CheckItem(Product product, int quantity)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }
            
            Code = product.Code;
            Price = product.Price;
            Quantity = quantity;
            TotalPrice = TotalPriceWithoutDiscount;
        }
        
        public string Code { get; }

        public int Quantity { get; private set; }

        public decimal Price { get; }

        public decimal TotalPrice { get; private set; }
        
        public decimal TotalPriceWithoutDiscount => Quantity * Price;

        public bool DiscountApplied => TotalPrice != TotalPriceWithoutDiscount;

        public void AddItems(int quantity)
        {
            Quantity += quantity;
            TotalPrice = TotalPriceWithoutDiscount;
        }
        
        public void SetTotalPrice(decimal newTotalPrice)
        {
            TotalPrice = newTotalPrice;
        }
        
        public void TryApplyDiscount(Discount discount)
        {
            discount.CalculatePriceWithDiscount(this);
        }
    }
}