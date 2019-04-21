using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.Extensions.DependencyInjection;

public class ChatRoomController : MonoBehaviour
{
	public InputField IPField;
	public InputField NameField;
	public InputField MessageField;

	private int m_Port = 4099;
	private ChatRoom m_ChatRoom;

	private void Start()
	{
		m_ChatRoom = GameMain.ServiceProvider.GetService<ChatRoom>();
	}

	public void Connect()
	{
		var host = IPField.text;
		var name = NameField.text;

		if (string.IsNullOrEmpty(name))
			return;

		m_ChatRoom.Connect(host, m_Port, name);
	}

	public void Send()
	{
		var message = MessageField.text;

		if (string.IsNullOrEmpty(message))
			return;

		m_ChatRoom.SendMessage(message);
		MessageField.text = string.Empty;
	}
}
