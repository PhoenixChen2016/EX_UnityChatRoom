using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ChatRoomLibrary
{
	public class SocketReceiveEventArgs : TcpClientEventArgs
	{
		public byte[] Data { get; internal set; }
		public int Length { get; internal set; }
	}
}
