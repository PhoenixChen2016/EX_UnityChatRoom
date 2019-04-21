using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChatRoomLibrary
{
	public class ChatRoomClient
	{
		private SocketClient m_SocketClient;

		public ChatRoomClient(string host, int port)
		{
			m_SocketClient = new SocketClient(host, port);

			m_SocketClient.ReceiveData += SocketClient_ReceiveData;
		}

		private void SocketClient_ReceiveData(object sender, SocketReceiveEventArgs e)
		{
			var request = Encoding.UTF8.GetString(e.Data, 0, e.Length);

			if (request.StartsWith("MESSAGE:", StringComparison.OrdinalIgnoreCase))
			{
				var tokens = request.Split(':');
				var name = tokens[1];
				var message = tokens[2];

				Console.WriteLine($"{name} said: {message}");
			}
		}

		public ValueTask ConnectAsync()
		{
			return m_SocketClient.ConnectAsync();
		}

		public Task SendAsync(string message)
		{
			var buffer = Encoding.UTF8.GetBytes(message);

			return m_SocketClient.SendAsync(buffer, 0, buffer.Length);
		}
	}
}
