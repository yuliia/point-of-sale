namespace PointOfSale.Interfaces
{
    public interface IPointOfSaleTerminal
    {
        void Scan(string code, int quantity = 1);
        decimal GetTotal();
        
        //As a part of technical task I implemented it here, it won't affect functionality.
        //But it seems to me that it should not be called from this interface.
        void SetPrice(string code, int quantity, decimal price);

        void AddDiscount(string code, decimal initialDiscount = 0m);

        void CloseCheck();
    }
}