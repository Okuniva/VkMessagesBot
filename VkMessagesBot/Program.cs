using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace VkMessagesBot
{
	static class Program
	{
		private static readonly HttpClient Client = new HttpClient();

		private static readonly TelegramBotClient Bot = new TelegramBotClient(Consts.TelegramBotAccessToken);

		static void Main(string[] args)
		{
			var me = Bot.GetMeAsync().Result;
			Console.Title = me.Username;

			Bot.StartReceiving(Array.Empty<UpdateType>());
			Console.WriteLine($"Start listening for @{me.Username}");
			Console.ReadLine();
			Console.ReadLine();
			Bot.StopReceiving();
		}
	}
}
