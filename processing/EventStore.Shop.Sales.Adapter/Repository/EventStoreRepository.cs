using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.Shop.Sales.Messages.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EventStore.Shop.Sales.Adapter
{
    public class EventStoreRepository : IRepository
    {
        private readonly IEventStoreConnection _connection;
        public static string EventClrTypeHeader = "EventClrTypeName";

        public EventStoreRepository(IEventStoreConnection connection)
        {
            _connection = connection;
        }

        public IEnumerable<Event> Get(string id)
        {
            var streamEvents = new List<ResolvedEvent>();
            StreamEventsSlice currentSlice;
            long nextSliceStart = StreamPosition.Start;
            do
            {
                currentSlice = _connection.ReadStreamEventsForwardAsync(id, nextSliceStart, 200, false).Result;
                if (currentSlice.Status == SliceReadStatus.StreamNotFound)
                    throw new AggregateNotFoundException("Could not found aggregate id " + id);
                nextSliceStart = currentSlice.NextEventNumber;
                streamEvents.AddRange(currentSlice.Events);
            } while (!currentSlice.IsEndOfStream);
            return currentSlice.Events.Select(e =>
                DeserializeObject(e.OriginalEvent.Data, e.OriginalEvent.Metadata) as Event);
        }

        public IEnumerable<Event> Get(string id, int eventsToLoad)
        {
            long nextSliceStart = StreamPosition.Start;
            var currentSlice = _connection.ReadStreamEventsForwardAsync(id, nextSliceStart, eventsToLoad, false).Result;
            if (currentSlice.Status == SliceReadStatus.StreamNotFound)
                throw new AggregateNotFoundException("Could not found aggregate id " + id);
            return currentSlice.Events.Select(e =>
                DeserializeObject(e.OriginalEvent.Data, e.OriginalEvent.Metadata) as Event);
        }

        public async Task<bool> Set(string id, List<Event> history)
        {
            var eventData = history.Select(CreateEventData).ToArray();
            // Save using ExpectedVersion.Any to swallow silently WrongExpectedVersion error
            await _connection.AppendToStreamAsync(id, ExpectedVersion.Any, eventData);
            return true;
        }

        private static EventData CreateEventData(object @event)
        {
            IDictionary<string, string> metadata;
            var originalEventType = @event.GetType().Name;
            if (((Event)@event).Metadata != null)
            {
                metadata = ((Event)@event).Metadata;
                if (!metadata.ContainsKey("$correlationId"))
                    throw new Exception("The event metadata must contains a $correlationId");
                if (!metadata.ContainsKey(EventClrTypeHeader))
                    metadata.Add(EventClrTypeHeader, @event.GetType().AssemblyQualifiedName);
                // Remove the metadata from the event body
                var tmp = (IDictionary<string, object>)@event.ToDynamic();
                tmp.Remove("Metadata");
                @event = tmp;
            }
            else
                throw new Exception("The event must have a $correlationId present in its metadata");
            var eventDataHeaders = SerializeObject(metadata);
            var data = SerializeObject(@event);
            var eventData = new EventData(Guid.NewGuid(), originalEventType, true, data, eventDataHeaders);
            return eventData;
        }

        private static byte[] SerializeObject(object obj)
        {
            var jsonObj = JsonConvert.SerializeObject(obj);
            var data = Encoding.UTF8.GetBytes(jsonObj);
            return data;
        }

        private static object DeserializeObject(byte[] data, byte[] metadata)
        {
            try
            {
                var dict = DeserializeObject<Dictionary<string, string>>(metadata);
                if (!dict.ContainsKey("$correlationId"))
                    throw new Exception("The metadata must contains a $correlationId");
                var bodyString = Encoding.UTF8.GetString(data);
                var o1 = JObject.Parse(bodyString);
                var o2 = JObject.Parse(JsonConvert.SerializeObject(new { metadata = dict }));
                o1.Merge(o2, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
                return JsonConvert.DeserializeObject(o1.ToString(),
                    Type.GetType(DeserializeObject<Dictionary<string, string>>(metadata)[EventClrTypeHeader]));
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static T DeserializeObject<T>(byte[] data)
        {
            return (T)(DeserializeObject(data, typeof(T).AssemblyQualifiedName));
        }

        private static object DeserializeObject(byte[] data, string typeName)
        {
            try
            {
                var jsonString = Encoding.UTF8.GetString(data);
                return JsonConvert.DeserializeObject(jsonString, Type.GetType(typeName));
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
