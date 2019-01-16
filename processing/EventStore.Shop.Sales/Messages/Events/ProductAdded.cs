using System.Collections.Generic;

namespace EventStore.Shop.Sales.Messages.Events
{
    public class ProductAdded : Event
    {
        public string Id { get; }
        public string Name { get; }
        public decimal Cost { get; }
        public IDictionary<string, string> Metadata { get; }

        public ProductAdded(string id, string name, decimal cost, IDictionary<string, string> metadata)
        {
            Id = id;
            Name = name;
            Cost = cost;
            Metadata = metadata;
        }
    }
}
