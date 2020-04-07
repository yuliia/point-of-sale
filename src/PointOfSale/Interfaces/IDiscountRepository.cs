using PointOfSale.Models;

namespace PointOfSale.Interfaces
{
    public interface IDiscountRepository
    {
        Discount GetDiscount(string code);
        void AddDiscount(Discount discount);

        void UpdateDiscount(Discount discount);
    }
}