using System;
using System.Collections.Generic;
using EventStore.Shop.Sales.Messages.Commands;
using EventStore.Shop.Sales.Messages.Events;

namespace EventStore.Shop.Sales.Adapter
{
    public class Handlers : IHandle<CreateBasket>, IHandle<AddProduct>
    {
        private readonly IRepository _repository;

        public Handlers(IRepository repository)
        {
            _repository = repository;
        }

        public List<Event> Handle(CreateBasket command)
        {
            throw new NotImplementedException();
        }

        public List<Event> Handle(AddProduct command)
        {
            throw new NotImplementedException();
        }
    }
}
