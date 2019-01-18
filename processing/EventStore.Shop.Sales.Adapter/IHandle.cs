using System.Collections.Generic;
using EventStore.Shop.Sales.Messages.Commands;
using EventStore.Shop.Sales.Messages.Events;

namespace EventStore.Shop.Sales.Adapter
{
    public interface IHandle<in TMessage> where TMessage : Command
    {
        List<Event> Handle(TMessage command);
    }
}
