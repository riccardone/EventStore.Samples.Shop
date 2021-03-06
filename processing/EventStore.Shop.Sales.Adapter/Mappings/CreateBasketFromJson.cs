﻿using System.Collections.Generic;
using EventStore.Shop.Sales.Messages.Commands;
using Newtonsoft.Json;

namespace EventStore.Shop.Sales.Adapter.Mappings
{
    public class CreateBasketFromJson : Command
    {
        public string Id { get; }
        public IDictionary<string, string> Metadata { get; }

        public CreateBasketFromJson(string bodyAsJson, string metadataAsJson)
        {
            var body = JsonConvert.DeserializeObject<dynamic>(bodyAsJson);
            var metadata = JsonConvert.DeserializeObject<IDictionary<string, string>>(metadataAsJson);
            // TODO
            Metadata = metadata;
        }
    }
}
