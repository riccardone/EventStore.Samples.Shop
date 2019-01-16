using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;

namespace EventStore.Shop.Sales.Adapter
{
    public interface IConnectionBuilder
    {
        UserCredentials Credentials { get; }
        IEventStoreConnection Build();
    }
}
