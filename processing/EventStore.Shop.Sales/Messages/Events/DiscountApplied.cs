using System.Collections.Generic;

namespace EventStore.Shop.Sales.Messages.Events
{
    public class DiscountApplied : Event
    {
        public string Id { get; }
        public string OfferName { get; }
        public string ProductIdDiscounted { get; }
        public decimal DiscountAmount { get; }
        public IDictionary<string, string> Metadata { get; }

        public DiscountApplied(string id, string offerName, string productIdDiscounted, decimal discountAmount, IDictionary<string, string> metadata)
        {
            Id = id;
            OfferName = offerName;
            ProductIdDiscounted = productIdDiscounted;
            DiscountAmount = discountAmount;
            Metadata = metadata;
        }
    }
}
