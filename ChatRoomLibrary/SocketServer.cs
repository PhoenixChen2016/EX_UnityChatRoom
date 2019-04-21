using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ChatRoomLibrary
{
	class SocketServer
	{
		private static byte[] _CheckBuffer = new byte[1];

		private CancellationTokenSource m_AcceptCancelSource;
		private Task m_AcceptTask;
		private byte[] m_Buffer = new byte[1024];
		private HashSet<TcpClient> m_Clients = new HashSet<TcpClient>();
		private TcpListener m_Listener;
		private IPAddress m_ListenIP;
		private int m_Port;
		private TcpClient[] m_ReadonlyClients;
		private CancellationTokenSource m_ReceiveCancelSource;
		private Task m_ReceiveTask;

		public event EventHandler<SocketReceiveEventArgs> ReceiveData;
		public event EventHandler<TcpClientEventArgs> ClientAdded;
		public event EventHandler<TcpClientEventArgs> ClientRemoved;

		public SocketServer(IPAddress listenIP, int port)
		{
			m_ListenIP = listenIP;
			m_Port = port;
		}

		public SocketServer(int port) : this(IPAddress.Any, port)
		{

		}

		public void Listen()
		{
			m_Listener = new TcpListener(m_ListenIP, m_Port);
			m_Listener.Start();

			m_AcceptCancelSource = new CancellationTokenSource();

			m_AcceptTask = Task.Factory.StartNew(
				() =>
				{
					while (true)
					{
						var acceptedClient = m_Listener.AcceptTcpClient();

						lock (m_Clients)
						{
							m_Clients.Add(acceptedClient);
							m_ReadonlyClients = m_Clients.ToArray();

							ClientAdded?.Invoke(
								this,
								new TcpClientEventArgs
								{
									Client = acceptedClient
								});

							if (m_Clients.Count == 1)
								Receive();
						}
					}
				},
				m_AcceptCancelSource.Token,
				TaskCreationOptions.LongRunning,
				TaskScheduler.Current);
		}

		private static bool CheckConnected(TcpClient client)
		{
			if (!client.Connected)
				return false;

			try
			{
				if (client.Client.Poll(0, SelectMode.SelectRead))
				{
					if (client.Client.Receive(_CheckBuffer, SocketFlags.Peek) == 0)
						return false;
				}
			}
			catch
			{
				return false;
			}

			return true;
		}

		private void Receive()
		{
			m_ReceiveCancelSource = new CancellationTokenSource();

			m_ReceiveTask = Task.Factory.StartNew(
				() =>
				{
					var buffer = new byte[1024];

					while (true)
					{
						foreach (var client in m_ReadonlyClients)
							if (!(CheckConnected(client) && ReceiveMessage(client)))
								RemoveClient(client);

						Thread.Sleep(1);
					}
				},
				m_ReceiveCancelSource.Token,
				TaskCreationOptions.LongRunning,
				TaskScheduler.Current);
		}

		private bool ReceiveMessage(TcpClient client)
		{
			try
			{
				if (client.Available > 0)
				{
					var stream = client.GetStream();

					var readLength = stream.Read(m_Buffer, 0, m_Buffer.Length);

					if (readLength > 0)
					{
						ReceiveData?.Invoke(
							this,
							new SocketReceiveEventArgs
							{
								Client = client,
								Data = m_Buffer,
								Length = readLength
							});
					}
				}

				return true;
			}
			catch
			{
				return false;
			}
		}

		private void RemoveClient(TcpClient client)
		{
			lock (m_Clients)
			{
				if (m_Clients.Contains(client))
				{
					m_Clients.Remove(client);
					m_ReadonlyClients = m_Clients.ToArray();

					ClientRemoved?.Invoke(
						this,
						new TcpClientEventArgs
						{
							Client = client
						});
				}
			}
		}

		public ValueTask BroadcastAsync(TcpClient sender, byte[] data)
		{
			Task.WaitAll(
				m_Clients
					.Where(client => client != sender)
					.Select(client => client.GetStream().WriteAsync(data, 0, data.Length))
					.ToArray());

			return new ValueTask(Task.CompletedTask);
		}
	}
}
