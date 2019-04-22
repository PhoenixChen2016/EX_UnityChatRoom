using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Reactive.Linq;

namespace ChatRoomLibrary
{
	public class ChatRoomServer : IDisposable
	{
		private SocketServer m_SocketServer;
		private ConcurrentDictionary<TcpClient, string> m_ClientNames = new ConcurrentDictionary<TcpClient, string>();
		private bool m_Disposed;

		public ChatRoomServer(int port)
		{
			m_SocketServer = new SocketServer(port);

			m_SocketServer.Listen();

			m_SocketServer.ReceiveData += SocketServer_ReceiveData;
			m_SocketServer.ClientRemoved += SocketServer_ClientRemoved;
		}

		~ChatRoomServer()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!m_Disposed)
			{
				m_SocketServer.Dispose();
				m_SocketServer = null;



				m_Disposed = true;
			}
		}

		private void SocketServer_ClientRemoved(object sender, TcpClientEventArgs e)
		{
			m_ClientNames.TryRemove(e.Client, out var _);
		}

		private void SocketServer_ReceiveData(object sender, SocketReceiveEventArgs e)
		{
			var requestMessage = Encoding.UTF8.GetString(e.Data, 0, e.Length);

			Console.WriteLine($"Text: {requestMessage}");

			if (requestMessage.StartsWith("LOGINNAME:", StringComparison.OrdinalIgnoreCase))
			{
				var tokens = requestMessage.Split(':');
				m_ClientNames.TryAdd(e.Client, tokens[1]);

				Console.WriteLine("...and the client name is: " + tokens[1]);
			}
			else if (requestMessage.StartsWith("BROADCAST:", StringComparison.OrdinalIgnoreCase))
			{
				var tokens = requestMessage.Split(':');
				var message = tokens[1];

				var broadcastMessage = $"MESSAGE:{m_ClientNames[e.Client]}:{message}";
				var buffer = Encoding.UTF8.GetBytes(broadcastMessage);

				m_SocketServer.BroadcastAsync(e.Client, buffer);
			}
		}
	}
}
