using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.Shop.Sales.Adapter.Mappings;
using EventStore.Shop.Sales.Messages.Commands;
using EventStore.Shop.Sales.Messages.Events;

namespace EventStore.Shop.Sales.Adapter
{
    public class EndPoint
    {
        private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();
        private readonly IRepository _repository;
        private readonly IConnectionBuilder _connectionBuilder;
        private IEventStoreConnection _connection;
        private readonly Handlers _handlers;
        private const string InputStream = "input";
        private const string PersistentSubscriptionGroup = "shop-processors";
        private readonly Dictionary<string, Func<string[], Command>> _deserialisers;
        private readonly Dictionary<string, Func<object, IEnumerable<Event>>> _eventHandlerMapping;

        public EndPoint(IRepository repository, IConnectionBuilder connectionBuilder)
        {
            _deserialisers = CreateDeserialisersMapping();
            _eventHandlerMapping = CreateEventHandlerMapping();
            _repository = repository;
            _connectionBuilder = connectionBuilder;
            _handlers = new Handlers(repository);
        }

        public async Task<bool> Start()
        {
            try
            {
                _connection = _connectionBuilder.Build();
                _connection.Connected += _connection_Connected;
                _connection.Disconnected += _connection_Disconnected;
                _connection.ErrorOccurred += _connection_ErrorOccurred;
                _connection.Closed += _connection_Closed;
                _connection.Reconnecting += _connection_Reconnecting;
                _connection.AuthenticationFailed += _connection_AuthenticationFailed;
                await _connection.ConnectAsync();
                Log.Info($"Listening from '{InputStream}' stream");
                Log.Info($"Joined '{PersistentSubscriptionGroup}' group");
                Log.Info($"EndPoint started");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        public bool Stop()
        {
            _connection.Close();
            return true;
        }

        private Task EventAppeared(EventStorePersistentSubscriptionBase eventStorePersistentSubscriptionBase, ResolvedEvent resolvedEvent)
        {
            try
            {
                Process(resolvedEvent.Event.EventType, resolvedEvent.Event.Metadata, resolvedEvent.Event.Data);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                eventStorePersistentSubscriptionBase.Fail(resolvedEvent, PersistentSubscriptionNakEventAction.Park,
                    ex.GetBaseException().Message);
            }
            return Task.CompletedTask;
        }

        private void Process(string eventType, byte[] metadata, byte[] data)
        {
            if (!_deserialisers.ContainsKey(eventType))
                return;

            var command = _deserialisers[eventType](new[]
            {
                Encoding.UTF8.GetString(metadata),
                Encoding.UTF8.GetString(data)
            });

            if (command == null)
            {
                Log.Error($"Message format not recognised! EventType: {eventType}");
                return;
            }

            foreach (var key in _eventHandlerMapping.Keys)
            {
                if (!eventType.EndsWith(key))
                    continue;
                _repository.Set(command.Metadata["$correlationId"], _eventHandlerMapping[key](command));
                Log.Debug($"Handled '{eventType}' AggregateId: {command.Metadata["$correlationId"]}");
                return;
            }
            throw new Exception($"I can't find any handler for {eventType}");
        }

        private static Dictionary<string, Func<string[], Command>> CreateDeserialisersMapping()
        {
            return new Dictionary<string, Func<string[], Command>>
            {
                {"CreateBasket", ToCreateBasketFromJson},
                {"AddProduct", ToAddProductFromJson}
            };
        }
        private Dictionary<string, Func<object, IEnumerable<Event>>> CreateEventHandlerMapping()
        {
            return new Dictionary<string, Func<object, IEnumerable<Event>>>
            {
                {"CreateBasket", o => _handlers.Handle(o as CreateBasket)},
                {"AddProduct", o => _handlers.Handle(o as AddProduct)}
            };
        }

        private static Command ToCreateBasketFromJson(string[] arg)
        {
            return new CreateBasketFromJson(arg[1], arg[0]);
        }

        private static Command ToAddProductFromJson(string[] arg)
        {
            return new AddProductFromJson(arg[1], arg[0]);
        }

        private void _connection_AuthenticationFailed(object sender, ClientAuthenticationFailedEventArgs e)
        {
            Log.Error($"EndpointConnection AuthenticationFailed: {e.Reason}");
        }

        private void _connection_Reconnecting(object sender, ClientReconnectingEventArgs e)
        {
            Log.Warn($"EndpointConnection Reconnecting...");
        }

        private void _connection_Closed(object sender, ClientClosedEventArgs e)
        {
            Log.Info($"EndpointConnection Closed: {e.Reason}");
        }

        private static void SubscriptionDropped(
            EventStorePersistentSubscriptionBase eventStorePersistentSubscriptionBase,
            SubscriptionDropReason subscriptionDropReason, Exception arg3)
        {
            Log.Error(subscriptionDropReason.ToString(), arg3);
        }

        private async Task CreateSubscription()
        {
            await _connection.CreatePersistentSubscriptionAsync(InputStream, PersistentSubscriptionGroup,
                PersistentSubscriptionSettings.Create().StartFromBeginning().DoNotResolveLinkTos(),
                _connectionBuilder.Credentials);
        }

        private static void _connection_ErrorOccurred(object sender, ClientErrorEventArgs e)
        {
            Log.Error($"EndpointConnection ErrorOccurred: {e.Exception.Message}");
        }

        private static void _connection_Disconnected(object sender, ClientConnectionEventArgs e)
        {
            Log.Error($"EndpointConnection Disconnected from {e.RemoteEndPoint}");
        }

        private async void _connection_Connected(object sender, ClientConnectionEventArgs e)
        {
            Log.Info($"EndpointConnection Connected to {e.RemoteEndPoint}");
            try
            {
                await CreateSubscription();
            }
            catch (Exception)
            {
                // already exist
            }
            await Subscribe();
        }

        private async Task Subscribe()
        {
            await _connection.ConnectToPersistentSubscriptionAsync(InputStream, PersistentSubscriptionGroup, EventAppeared, SubscriptionDropped);
        }
    }
}
