using System.Collections.Generic;

namespace EventStore.Shop.Sales.Messages
{
    public interface Message
    {
        string Id { get; }
        IDictionary<string, string> Metadata { get; }
    }
}
