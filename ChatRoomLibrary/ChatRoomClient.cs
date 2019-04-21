using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Subjects;
using System.Reactive;
using System.Reactive.Linq;

namespace ChatRoomLibrary
{
	public class ChatRoomClient
	{
		private SocketClient m_SocketClient;

		public event EventHandler<ChatMessageEventArgs> Receive;

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

				Receive?.Invoke(
					this,
					new ChatMessageEventArgs
					{
						Message = new ChatMessage
						{
							Name = name,
							Text = message
						}
					});
			}
		}

		public void Connect()
		{
			m_SocketClient.Connect();
		}

		public void Send(string message)
		{
			var buffer = Encoding.UTF8.GetBytes(message);

			m_SocketClient.Send(buffer, 0, buffer.Length);
		}
	}
}
