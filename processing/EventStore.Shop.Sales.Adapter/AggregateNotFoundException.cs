using System;

namespace EventStore.Shop.Sales.Adapter
{
    public class AggregateNotFoundException : Exception
    {
        public AggregateNotFoundException(string message)
            : base(message)
        {
        }
    }
}
