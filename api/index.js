const express = require('express')
var bodyParser = require("body-parser")
var esClient = require('node-eventstore-client')
var ConnectionFactory = require('./connectionFactory')
var Sender = require('./sender')
var _validation = require('./validation')
const uuidv4 = require('uuid/v4')
var _connFactory = new ConnectionFactory(esClient, "admin", "changeit", "tcp://eventstore:1113", "shop-api")
var _sender = new Sender(_connFactory, "shop-input", uuidv4)

const app = express()
app.use(bodyParser.json())
const port = 3000

app.get('/', (req, res) => res.send('ok'))

app.post("/shop/api/v1", function (req, res) {
    if (_validation.requestNotValid(req)) {
        return res.status(400).send("Request body not valid");
    }
    _sender.send(req.body, "ShopDataReceived").then(function (result) {
        res.send();
    }).catch(error => {
        console.error(error.message || error);
        res
            .status(500)
            .send("There is a technical problem and the data has not been stored");
    });
})

app.listen(port, () => console.log(`EventStore Shop-Api service listening on port ${port}!`))