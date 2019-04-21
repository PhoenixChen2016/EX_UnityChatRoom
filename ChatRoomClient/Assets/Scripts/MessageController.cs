using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.Extensions.DependencyInjection;
using System.Reactive.Linq;
using System;

public class MessageController : MonoBehaviour
{
	private Text m_Message;

	private void Start()
	{
		m_Message = GetComponent<Text>();

		var chatRoom = GameMain.ServiceProvider.GetService<ChatRoom>();

		chatRoom.Receive += ChatRoom_Receive;
	}

	private void ChatRoom_Receive(object sender, ChatRoomLibrary.ChatMessageEventArgs e)
	{
		m_Message.text += $"{e.Message.Name} sad: {e.Message.Text}" + Environment.NewLine;
	}
}
