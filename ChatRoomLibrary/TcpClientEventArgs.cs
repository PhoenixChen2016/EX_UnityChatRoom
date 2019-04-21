using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ChatRoomLibrary
{
	public class TcpClientEventArgs
	{
		public TcpClient Client { get; internal set; }
	}
}
