using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ChatRoomLibrary;

public class ChatRoom
{
	private ChatRoomClient m_Client;

	public event EventHandler<ChatMessageEventArgs> Receive;

	public void Connect(string host, int port, string name)
	{
		m_Client = new ChatRoomClient(host, port);

		m_Client.Connect();


		m_Client.Receive += (sender, args) =>
		{
			Receive?.Invoke(this, args);
		};

		m_Client.SendName(name);
	}

	public void SendMessage(string message)
	{
		m_Client.SendMessage(message);
	}
}
