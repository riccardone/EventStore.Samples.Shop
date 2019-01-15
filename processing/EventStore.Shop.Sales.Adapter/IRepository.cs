using System.Collections.Generic;
using EventStore.Shop.Sales.Messages.Events;

namespace EventStore.Shop.Sales.Adapter
{
    public interface IRepository
    {
        List<Event> GetById(string correlationId);
        bool Save(List<Event> history);
    }
}
