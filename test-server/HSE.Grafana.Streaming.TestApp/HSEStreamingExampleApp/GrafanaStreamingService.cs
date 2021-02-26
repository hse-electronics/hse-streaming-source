using Fleck;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using Xamarin.Essentials;

namespace HSEStreamingExampleApp
{
	public sealed class GrafanaStreamingService : IDisposable
	{
		private readonly List<IWebSocketConnection> _openSockets = new List<IWebSocketConnection>();

		public int OpenConnections => _openSockets.Count;

		public Quaternion OrientationData { get; private set; }

		public event EventHandler? OnSend;

		private readonly SensorSpeed _speed;
		private Timer? timer;
		private readonly int _sendingIntervalMS;
		bool webServerStarted = false;


		private readonly WebSocketServer wsServer;

		public GrafanaStreamingService(int sendingIntervalMS = 15)
		{
			_sendingIntervalMS = sendingIntervalMS;
			_speed = SensorSpeed.UI;
			wsServer = new WebSocketServer("ws://0.0.0.0:8181") { RestartAfterListenError = true };
			OrientationSensor.ReadingChanged += OrientationSensor_ReadingChanged;

		}
		private void OrientationSensor_ReadingChanged(object sender, OrientationSensorChangedEventArgs e)
		{
			OrientationData = e.Reading.Orientation;
		}

		private void StartWS()
		{
			if (!webServerStarted)
			{
				wsServer.Start(socket =>
				{
					socket.OnOpen = () =>
					{
						lock (_openSockets)
						{
							_openSockets.Add(socket);
						}
						Debug.WriteLine($"New WebSocket. Total: {_openSockets.Count}");
					};
					socket.OnClose = () =>
					{
						lock (_openSockets)
						{
							_openSockets.Remove(socket);
						}
						Debug.WriteLine($"WebSocket closed Total: {_openSockets.Count}");
					};
					socket.OnMessage = message => Debug.WriteLine("WebSocket: " + message);
				});
				webServerStarted = true;
			}
		}

		private void Send(object state)
		{
			var json = GenerateJson();
			OnSend?.Invoke(this, EventArgs.Empty);
			lock (_openSockets)
			{
				foreach (IWebSocketConnection socket in _openSockets)
				{
					socket.Send(json);
				}
			}
		}



		private string GenerateJson()
		=> JsonConvert.SerializeObject(new
		{
			time = NowUnixTime(),
			x = OrientationData.X,
			y = OrientationData.Y,
			z = OrientationData.Z
		});




		public void Start()
		{
			StartWS();
			OrientationSensor.Start(_speed);
			timer = new Timer(Send, null, 0, _sendingIntervalMS);
		}

		public void Stop()
		{
			timer?.Dispose();
			OrientationSensor.Stop();
		}

		public void Dispose()
		{
			Stop();
			wsServer.Dispose();
			OrientationSensor.ReadingChanged -= OrientationSensor_ReadingChanged;
		}
		private static long NowUnixTime() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
	}


}
