using System;
using ChatRoomLibrary;

namespace ServerTest
{
	class Program
	{
		static void Main(string[] args)
		{
			var server = new ChatRoomServer(4099);

			Console.ReadKey();

			server.Dispose();

			Console.ReadKey();
		}
	}
}
