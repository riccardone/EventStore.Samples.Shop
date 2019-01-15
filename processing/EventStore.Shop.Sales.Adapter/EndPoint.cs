using System;

namespace EventStore.Shop.Sales.Adapter
{
    public class EndPoint
    {
        private readonly IRepository _repository;
        private Handlers _handlers;

        public EndPoint(IRepository repository)
        {
            _repository = repository;
            _handlers = new Handlers(repository);
        }

        public bool Start()
        {
            throw new NotImplementedException();
        }

        public bool Stop()
        {
            throw new NotImplementedException();
        }
    }
}
