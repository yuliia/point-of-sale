using PointOfSale.Models;

namespace PointOfSale.Interfaces
{
    public interface IDiscountCardService
    {
        void HandleCheckClosed(CheckClosed closed);
    }
}