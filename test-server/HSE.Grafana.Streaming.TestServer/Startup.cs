using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace HSE.Grafana.Streaming.TestServer
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration config)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();
			app.UseWebSockets();

			var pingTimeoutS = config.GetValue<int?>("SERVER_TIMEOUT_SECONDS") ?? 30;
			var pingTimeoutMS = pingTimeoutS > 0 && pingTimeoutS < int.MaxValue / 1000 ? pingTimeoutS * 1000 : 0;

			//Don't use this for production, implement a BackgroundService which handles the websockets
			app.Use(async (ctx, next) =>
			{
				//This violates the 'dont use long running Task in Middleware' paradigma, dont use for production

				if (ctx.WebSockets.IsWebSocketRequest)
				{
					await new WebSocketHandler(ctx, pingTimeoutMS).RunAsync();
				}
				else
				{
					await next();
				}


			});


			app.UseEndpoints(endpoints =>
			{
				endpoints.MapGet("/", async context =>
				{
					await context.Response.WriteAsync($"Connect via WebSocket: ws://localhost:8181");
				});
			});
		}
	}
	internal class WebSocketHandler
	{
		int curVal;
		private readonly CancellationTokenSource _cts;
		private readonly int _pingTimeoutMS;
		private readonly HttpContext _ctx;

		public WebSocketHandler(HttpContext ctx, int pingTimeoutMS)
		{
			_pingTimeoutMS = pingTimeoutMS;
			_cts = new CancellationTokenSource();
			_ctx = ctx;

		}
		public async Task RunAsync()
		{
			try
			{
				var webSocket = await _ctx.WebSockets.AcceptWebSocketAsync();
				if (_pingTimeoutMS > 0)
					_ = Task.Run(() => StartPing(webSocket));//Not a safe fire and forget, dont use in production
				while (webSocket.State == WebSocketState.Open && !_cts.Token.IsCancellationRequested)
				{
					await SendRandomData(webSocket);
					await Task.Delay(20);
				}
				if (webSocket.State <= WebSocketState.Open)
					webSocket.Abort();
				Console.WriteLine($"WebSocket stopped: {webSocket.CloseStatus} ({webSocket.CloseStatusDescription})");
			}
			catch (Exception e)
			{
				Console.WriteLine($"ERROR: {e.Message}");
			}
		}

		private void StartPing(WebSocket webSocket)
		{
			var buffer = ArrayPool<byte>.Shared.Rent(4);
			while (webSocket.State == WebSocketState.Open)
			{
				//this is a nasty but the easiest way without setting the websocket state to abborted
				//don't use this for production
				if (!webSocket.ReceiveAsync(buffer, CancellationToken.None).Wait(_pingTimeoutMS))
				{
					webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, $"No ping recived from client in {_pingTimeoutMS / 1000}s", CancellationToken.None);
					break;
				}
			}
			_cts.Cancel();
		}


		private async Task SendRandomData(WebSocket sckt)
		{
			if (++curVal > 360)
				curVal = 0;
			var r = curVal * Math.PI / 180;

			var ser = JsonSerializer.SerializeToUtf8Bytes(new
			{
				time = ToUnixTime(DateTime.Now),
				sin = Math.Sin(r),
				cos = Math.Cos(r),
				sin180 = Math.Sin((curVal + 180) * Math.PI / 180),
				cos180 = Math.Cos((curVal + 180) * Math.PI / 180)
			});
			await sckt.SendAsync(ser, WebSocketMessageType.Text, true, CancellationToken.None);


		}

		private static long ToUnixTime(DateTime t)
		{
			return ((DateTimeOffset)t.ToUniversalTime()).ToUnixTimeMilliseconds();
		}
	}
}
