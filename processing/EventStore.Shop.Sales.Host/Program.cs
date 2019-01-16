using System;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using EventStore.Shop.Sales.Adapter;

namespace EventStore.Shop.Sales.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            var endPoint = new EndPoint(new EventStoreRepository(new ConnectionBuilder(new Uri("tcp://eventstore:1113"),
                ConnectionSettings.Default, "shop", new UserCredentials("admin", "changeit")).Build()));
            endPoint.Start();

            Console.WriteLine("Press enter to leave the program");
            Console.ReadLine();
        }
    }
}
