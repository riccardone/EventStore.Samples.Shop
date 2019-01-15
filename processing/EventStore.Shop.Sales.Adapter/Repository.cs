using System;
using System.Collections.Generic;
using EventStore.Shop.Sales.Messages.Events;

namespace EventStore.Shop.Sales.Adapter
{
    public class Repository : IRepository
    {
        public List<Event> GetById(string correlationId)
        {
            throw new NotImplementedException();
        }

        public bool Save(List<Event> history)
        {
            throw new NotImplementedException();
        }
    }
}
