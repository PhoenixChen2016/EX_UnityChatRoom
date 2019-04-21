using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Extensions.DependencyInjection;
using System;
using ChatRoomLibrary;

public static class GameMain
{
	public static IServiceProvider ServiceProvider { get; private set; }

	[RuntimeInitializeOnLoadMethod]
	public static void GameLoaded()
	{
		var services = new ServiceCollection();

		ConfigureServices(services);

		ServiceProvider = services.BuildServiceProvider();
	}

	private static void ConfigureServices(IServiceCollection services)
	{
		services.AddSingleton<ChatRoom>();
	}
}
