using System;
using System.Threading.Tasks;
using ChatRoomLibrary;

namespace ClientTest
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var host = "localhost";
			var port = 4099;

			Console.WriteLine("<Please enter the name...>");
			var name = Console.ReadLine();

			var client = new ChatRoomClient(host, port);

			await client.ConnectAsync();

			await client.SendNameAsync(name);

			Console.WriteLine("輸入字串可以送出訊息");

			while (true)
			{
				var text = Console.ReadLine();
				await client.SendMessageAsync(text);

				Console.WriteLine($"訊息： {text} 已送出！");
			}
		}
	}
}
