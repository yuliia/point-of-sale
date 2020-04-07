using System;

namespace PointOfSale.Models
{
    public class VolumeDiscount : Discount
    {
        public VolumeDiscount(string code, int quantity, decimal price) : base(code)
        {
            Quantity = quantity;
            Price = price;
        }

        public int Quantity { get; }

        public decimal Price { get; }
        
        public override void CalculatePriceWithDiscount(CheckItem item)
        {
            if (item.Code != Code)
            {
                throw new InvalidOperationException($"Can't apply volume discount for {Code} to {item.Code}");
            }

            if (item.Quantity < Quantity)
            {
                return;
            }
            
            var volumesCount = (int)Math.Floor(item.Quantity / (decimal)Quantity);
            var withoutDiscount = item.Quantity % Quantity;
            var total = volumesCount * Price + withoutDiscount * item.Price;
            item.SetTotalPrice(total);
        }
    }
}