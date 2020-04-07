using PointOfSale.Interfaces;
using PointOfSale.Models;

namespace PointOfSale.Services
{
    public class DiscountCardService : IDiscountCardService
    {
        private readonly IDiscountRepository _repository;

        public DiscountCardService(IDiscountRepository repository)
        {
            _repository = repository;
        }

        public void HandleCheckClosed(CheckClosed closed)
        {
            var discount = _repository.GetDiscount(closed.DiscountCardCode) as CumulativeDiscount;

            discount?.AccumulateAmount(closed.Amount);
            
            _repository.UpdateDiscount(discount);
        }
    }
}