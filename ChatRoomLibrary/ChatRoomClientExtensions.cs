using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChatRoomLibrary
{
	public static class ChatRoomClientExtensions
	{
		public static void SendName(this ChatRoomClient client, string name)
		{
			var request = $"LOGINNAME:{name}";

			client.Send(request);
		}

		public static void SendMessage(this ChatRoomClient client, string message)
		{
			client.Send($"BROADCAST:{message}");
		}
	}
}
