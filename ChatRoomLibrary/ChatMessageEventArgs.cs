using System;

namespace ChatRoomLibrary
{
	public class ChatMessageEventArgs : EventArgs
	{
		public ChatMessage Message { get; internal set; }
	}
}
