# hse Streaming Source

This is a simple streaming datasource using WebSockets. It is adaptive: The first packet (as JSON) received via WebSocket will set the structure of the metrcis sent to Grafana. 

# Configuration

Server: The WebSocket server, e.g. `ws://localhost:8080`
Capacity: The maximum amount of data in the stream, e.g. `1000`
Server Timeout (seconds): The interval on which the client should send a ping to the server. If its 0 no ping will be sent