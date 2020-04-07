using System;

namespace PointOfSale.Models
{
    public class CumulativeDiscount : Discount
    {
        public CumulativeDiscount(string code) : base(code)
        {
        }
        
        public decimal Percent
        {
            get
            {
                if (AccumulatedAmount > 9999)
                {
                    return 0.07m;
                }

                if (AccumulatedAmount > 4999)
                {
                    return 0.05m;
                }

                if (AccumulatedAmount > 1999)
                {
                    return 0.03m;
                }

                if (AccumulatedAmount > 999)
                {
                    return 0.01m;
                }

                return 0m;
            }
        }
        
        public decimal AccumulatedAmount { get; private set; }
        
        public override void CalculatePriceWithDiscount(CheckItem item)
        {
            if (item.DiscountApplied)
            {
                return;
            }

            var total = item.TotalPrice - item.TotalPrice * Percent;
            
            item.SetTotalPrice(total);
        }

        public void AccumulateAmount(decimal amount)
        {
            AccumulatedAmount += amount;
        }
    }
}