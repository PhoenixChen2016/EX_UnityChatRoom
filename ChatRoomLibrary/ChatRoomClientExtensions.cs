using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ChatRoomLibrary
{
	public static class ChatRoomClientExtensions
	{
		public static Task SendNameAsync(this ChatRoomClient client, string name)
		{
			var request = $"LOGINNAME:{name}";

			return client.SendAsync(request);
		}

		public static Task SendMessageAsync(this ChatRoomClient client, string message)
		{
			return client.SendAsync($"BROADCAST:{message}");
		}
	}
}
