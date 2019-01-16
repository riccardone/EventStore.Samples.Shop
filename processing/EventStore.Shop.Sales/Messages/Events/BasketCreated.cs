using System.Collections.Generic;

namespace EventStore.Shop.Sales.Messages.Events
{
    public class BasketCreated : Event
    {
        public string Id { get; }
        public string ClientId { get; }
        public IDictionary<string, string> Metadata { get; }

        public BasketCreated(string id, string clientId, IDictionary<string, string> metadata)
        {
            Id = id;
            ClientId = clientId;
            Metadata = metadata;
        }
    }
}
