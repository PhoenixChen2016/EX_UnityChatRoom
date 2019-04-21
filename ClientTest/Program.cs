using System;
using System.Threading.Tasks;
using ChatRoomLibrary;
using System.Reactive.Linq;

namespace ClientTest
{
	class Program
	{
		static void Main(string[] args)
		{
			var host = "localhost";
			var port = 4099;

			Console.WriteLine("<Please enter the name...>");
			var name = Console.ReadLine();

			var client = new ChatRoomClient(host, port);

			client.Receive += (sender, eventArgs) =>
			{
				Console.WriteLine($"{eventArgs.Message.Name} sad: {eventArgs.Message.Text}");
			};
			client.Connect();

			client.SendName(name);

			Console.WriteLine("輸入字串可以送出訊息");

			while (true)
			{
				var text = Console.ReadLine();
				client.SendMessage(text);

				Console.WriteLine($"訊息： {text} 已送出！");
			}
		}
	}
}
