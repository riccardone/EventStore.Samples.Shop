namespace EventStore.Shop.Sales.Messages.Commands
{
    public class CreateBasket : Command
    {
        public string ClientId { get; }
        public string Id { get; }

        public CreateBasket(string id, string clientId)
        {
            Id = id;
            ClientId = clientId;
        }
    }
}
