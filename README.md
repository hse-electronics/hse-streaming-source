# hse Streaming Source

This is a simple streaming datasource using WebSockets. It is adaptive: The first packet (as JSON) received via WebSocket will set the structure of the metrcis sent to Grafana. 


# Installation
Download latest release on https://hse-electronics.com 

# Demo & Compile
Linux, MacOS, Windows, doesn't matter as long as you have docker and docker-compose installed. To build and run simply do the following:
```BASH
git clone XXX
cd XXX
docker-compose up
```

This will build the plugin itself, put it in a Grafana Container which has some provisoning and it will run and build an example server written in C#. When everything is complete (the first run will take some minutes as the dependencies must load) just go to your favourite browser and type:
```
http://localhost:3000
```
Login with default user `admin` and password `admin` and voilá, you should see the working example.

The compiled version of the plugin can be found in grafana/volume/plugins

# Android Server
A much cooler example is the Android App which uses the Orientation Sensor. It's build with the experimental Blazor Mobile Binding. The pre built apk can be found on the bottom of this page. 

To use the app simply install the apk, turn on your WiFi, run the app and click on `START` You should see an IP-Address and a port. Choose the IP Address of your WiFi (e.g. 192.168.0.100) and enter this on top of the Grafana Dashboard in server like this:
`ws://192.168.0.100:8181` (Change the IP)

# Downloads
* Pre-Built hse Streaming Source Grafana Plugin
* Pre-Built Android App which uses the OrientationSensor