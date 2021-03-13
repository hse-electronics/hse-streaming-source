using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace HSEStreamingExampleApp
{
	public partial class GrafanaStreamingTest
	{

		private bool isSending = true;
		private GrafanaStreamingService GrafanaStreaming { get; } = new GrafanaStreamingService();
		private IEnumerable<string> IPAddresses { get; }
		private int Port { get; }

		public GrafanaStreamingTest()
		{
			var _ipAddresses = new List<string>();
			foreach (IPAddress address in Dns.GetHostAddresses(Dns.GetHostName()))
			{
				_ipAddresses.Add(address.ToString());
			}
			IPAddresses = _ipAddresses;
			Port = 8181;
		}

		protected override void OnInitialized()
		{
			GrafanaStreaming.OnSend += _streaming_OnSend;
			GrafanaStreaming.Start();
			base.OnInitialized();
		}

		private void _streaming_OnSend(object sender, EventArgs e)
		{
			InvokeAsync(() => StateHasChanged());
		}

		private void ToggleSending()
		{
			isSending = !isSending;
			if (isSending)
				GrafanaStreaming.Start();
			else
				GrafanaStreaming.Stop();
		}
	}
}
