module.exports = function (esClient, user, password, link, name) {
    var createEsConnection = () => {
        var esConnection = esClient.createConnection({ user: password }, link, name)
        esConnection.on('error', err =>
            console.error(`Error occurred on connection: ${err}`)
        )
        esConnection.on('closed', (reason) => {
            console.error(`Connection closed, reason: ${reason}`)
            console.error('dying...')
            process.exit(1)
        })
        esConnection.once('connected', (tcpEndPoint) => {
            console.info('Connected to eventstore at ' + tcpEndPoint.host + ':' + tcpEndPoint.port)
        })
        esConnection.connect().catch(err => console.error(err))
        return esConnection
    }

    return {
        createEsConnection: createEsConnection,
        createJsonEventData: esClient.createJsonEventData
    }
}
