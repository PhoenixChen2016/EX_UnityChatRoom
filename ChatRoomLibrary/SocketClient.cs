using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ChatRoomLibrary
{
	class SocketClient
	{
		private static byte[] _CheckBuffer = new byte[1];

		private byte[] m_Buffer = new byte[1024];
		private string m_Host;
		private int m_Port;
		private CancellationTokenSource m_ReceiveCancelSource;
		private Task m_ReceiveTask;
		private TcpClient m_TcpClient = new TcpClient();

		public event EventHandler<SocketReceiveEventArgs> ReceiveData;

		public SocketClient(string host, int port)
		{
			m_Host = host;
			m_Port = port;
		}

		public void Connect()
		{
			m_TcpClient.Connect(m_Host, m_Port);

			Receive();
		}

		public void Send(byte[] data, int offset, int count)
		{
			var stream = m_TcpClient.GetStream();

			stream.Write(data, offset, count);
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
						var ret = !(CheckConnected(m_TcpClient) && ReceiveMessage(m_TcpClient));

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
	}
}
