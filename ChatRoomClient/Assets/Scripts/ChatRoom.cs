using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ChatRoomLibrary;
using UnityEngine;
using System.Threading;

public class ChatRoom
{
	private ChatRoomClient m_Client;
	private IDisposable m_ClientReceiveEventSubscribe;

	public event EventHandler<ChatMessageEventArgs> Receive;

	public void Connect(string host, int port, string name)
	{
		m_Client = new ChatRoomClient(host, port);

		m_Client.Connect();

		var syncContext = SynchronizationContext.Current;
		m_ClientReceiveEventSubscribe = Observable.FromEventPattern<ChatMessageEventArgs>(
			h => m_Client.Receive += h,
			h => m_Client.Receive -= h)
			.Subscribe(e =>
			{
				syncContext.Post(state => Receive?.Invoke(this, e.EventArgs), null);
			});

		m_Client.SendName(name);
	}

	public void SendMessage(string message)
	{
		m_Client.SendMessage(message);
	}
}
