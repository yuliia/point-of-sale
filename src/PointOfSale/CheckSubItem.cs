using PointOfSale.Models;

namespace PointOfSale
{
    public class CheckSubItem
    {
        public int Quantity { get; set; }
        public PriceInfo PriceApplied { get; set; }

        /// <summary>
        /// Split specified quantity with specified price into separate sub item
        /// Leave original sub item with decreased quantity and price
        /// </summary>
        /// <param name="quantity"></param>
        /// <param name="price"></param>
        /// <param name="priceApplied"></param>
        /// <returns></returns>
        public CheckSubItem Fission(int quantity)
        {
            //todo check if quantity, price < Quantity, Price
            Quantity -= quantity;
            
            return new CheckSubItem
            {
                Quantity = quantity
            };
        }
    }
}