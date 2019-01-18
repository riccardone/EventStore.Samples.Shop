using System.Collections.Generic;
using System.Threading.Tasks;
using EventStore.Shop.Sales.Messages.Events;

namespace EventStore.Shop.Sales.Adapter
{
    public interface IRepository
    {
        IEnumerable<Event> Get(string id);
        IEnumerable<Event> Get(string id, int eventsToLoad);
        Task<bool> Set(string id, IEnumerable<Event> history);
    }
}
