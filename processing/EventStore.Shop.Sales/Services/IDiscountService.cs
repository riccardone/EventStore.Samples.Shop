using System.Collections.Generic;
using EventStore.Shop.Sales.Messages.Events;

namespace EventStore.Shop.Sales.Services
{
    public interface IDiscountService
    {
        void Apply(List<Event> history);
    }
}
