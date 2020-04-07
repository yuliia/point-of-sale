namespace PointOfSale.Models
{
    public class CheckClosed
    {
        public CheckClosed(string discountCardCode, decimal amount)
        {
            DiscountCardCode = discountCardCode;
            Amount = amount;
        }

        public string DiscountCardCode { get; }

        public decimal Amount { get; }
    }
}