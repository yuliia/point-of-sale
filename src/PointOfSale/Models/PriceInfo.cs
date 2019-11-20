using System;

namespace PointOfSale.Models
{
    public class PriceInfo
    {
        public PriceInfo(string code, int quantity, decimal price, PriceType type = PriceType.DefaultPrice, PriceUnit unit = PriceUnit.Money)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentNullException(nameof(code));
            }
            
            if (quantity <= 0)
            {
                throw new ArgumentException($"{nameof(quantity)} should be positive number.");
            }
            
            if (price <= 0)
            {
                throw new ArgumentException($"{nameof(price)} should be positive number.");
            }

            Code = code;
            Quantity = quantity;
            Price = price;
        }
        
        public string Code { get; }

        public int Quantity { get; }
        
        public decimal Price { get; }

        public PriceType Type { get; set; }

        public PriceUnit PriceUnit { get; set; }
        
        protected bool Equals(PriceInfo other)
        {
            return string.Equals(Code, other.Code) && Quantity == other.Quantity && Price == other.Price;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PriceInfo) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Code != null ? Code.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Quantity;
                hashCode = (hashCode * 397) ^ Price.GetHashCode();
                return hashCode;
            }
        }
    }

    public enum PriceUnit
    {
        Money = 0,
        Percents
    }

    public enum PriceType
    {
        DefaultPrice = 0,
        VolumeDiscount = 1,
        CumulativeDiscount = 2
    }

    public class CumulativeDiscount
    {
        public string Code { get; set; }

        public decimal AmountAccumulated { get; set; }
    }
}