namespace PointOfSale.Models
{
    public abstract class Discount
    {
        protected Discount(string code)
        {
            Code = code;
        }

        public string Code { get; }

        public abstract void CalculatePriceWithDiscount(CheckItem item);
    }
}