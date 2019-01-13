module.exports = function (connFactory, publishTo, uuidv4) {
    var _connFactory = connFactory
    var _conn = connFactory.createEsConnection()
    var _publishTo = publishTo || 'data-input'

    var send = function (message, eventType, publishTo) {
        if (!publishTo) {
            publishTo = _publishTo;
        }
        var eventData = toEventData(message)
        var event = _connFactory.createJsonEventData(eventData.eventId, eventData.data, eventData.metadata, eventType)
        return _conn.appendToStream(publishTo, esClient.expectedVersion.any, event)
    }

    var toEventData = function (msg) {
        var applies = msg.applies
        var source = msg.source
        delete msg.profile
        delete msg.applies
        delete msg.source
        var eventData = {
            eventId: uuidv4(),
            data: msg,
            metadata: { 'Applies': applies, 'Source': source }
        }
        return eventData
    }

    return { send: send }
}
