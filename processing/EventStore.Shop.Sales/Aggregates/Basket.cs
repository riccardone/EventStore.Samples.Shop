using System;
using System.Collections.Generic;
using EventStore.Shop.Sales.Messages.Commands;
using EventStore.Shop.Sales.Messages.Events;
using EventStore.Shop.Sales.Services;

namespace EventStore.Shop.Sales
{
    public class Basket
    {
        private Basket() { }

        public static List<Event> Create(CreateBasket cmd)
        {
            Ensure.NotNull(cmd, nameof(cmd));
            Ensure.NotNullOrEmpty(cmd.Id, nameof(cmd.Id));
            Ensure.NotNullOrEmpty(cmd.ClientId, nameof(cmd.ClientId));
            return new List<Event> {new BasketCreated(Guid.NewGuid().ToString(), cmd.Id, cmd.Id, cmd.ClientId)};
        }

        public static List<Event> Buy(List<Event> history, AddProduct cmd)
        {
            Ensure.NotNull(cmd, nameof(cmd));
            Ensure.NotNullOrEmpty(cmd.Name, nameof(cmd.Name));
            Ensure.Nonnegative(cmd.Cost, nameof(cmd.Cost));
            history.Add(new ProductAdded(Guid.NewGuid().ToString(), cmd.Id, cmd.CorrelationId, cmd.Name, cmd.Cost));
            return history;
        }

        public static List<Event> CheckOut(List<Event> history, IDiscountService discountService)
        {
            Ensure.NotNull(discountService, nameof(discountService));
            Ensure.NotNull(history, nameof(history));
            discountService.Apply(history);
            return history;
        }
    }
}
