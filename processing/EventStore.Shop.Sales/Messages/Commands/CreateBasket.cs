using System.Collections.Generic;

namespace EventStore.Shop.Sales.Messages.Commands
{
    public class CreateBasket : Command
    {
        public string ClientId { get; }
        public string Id { get; }
        public IDictionary<string, string> Metadata { get; }

        public CreateBasket(string id, string clientId, IDictionary<string, string> metadata)
        {
            Id = id;
            ClientId = clientId;
            Metadata = metadata;
        }
    }
}
