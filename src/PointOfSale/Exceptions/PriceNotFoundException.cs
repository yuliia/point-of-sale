using System;

namespace PointOfSale.Exceptions
{
    public class PriceNotFoundException : Exception
    {
        public string Code { get; }

        public PriceNotFoundException(string code)
        {
            Code = code;
        }
    }
}