using System;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using EventStore.Shop.Sales.Adapter;
using NLog;

namespace EventStore.Shop.Sales.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigureLogging();

            var connBuilderToGetInputData = new ConnectionBuilder(new Uri("tcp://eventstore:1113"),
                ConnectionSettings.Default, "shop-input", new UserCredentials("admin", "changeit"));
            var connectionForDomainEvents =
                EventStoreConnection.Create(ConnectionSettings.Default, new Uri("tcp://eventstore:1113"), "shop-processing");

            var endPoint = new EndPoint(new EventStoreRepository(connectionForDomainEvents), connBuilderToGetInputData);
            endPoint.Start().Wait();

            Console.WriteLine("Press enter to leave the program");
            Console.ReadLine();
            endPoint.Stop();
        }

        private static void ConfigureLogging()
        {
            var config = new NLog.Config.LoggingConfiguration();
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            LogManager.Configuration = config;
        }
    }
}
