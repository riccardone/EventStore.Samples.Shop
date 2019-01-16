using System.Collections.Generic;

namespace EventStore.Shop.Sales.Messages.Commands
{
    public class AddProduct : Command
    {
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public string Id { get; }
        public IDictionary<string, string> Metadata { get; }

        public AddProduct(string id, string name, decimal cost, IDictionary<string, string> metadata)
        {
            Metadata = metadata;
            Id = id;
            Name = name;
            Cost = cost;
        }
    }
}
