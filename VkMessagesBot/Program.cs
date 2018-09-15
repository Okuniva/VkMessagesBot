using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace VkMessagesBot
{
	static class Program
	{
		static readonly HttpClient Client = new HttpClient();

		static readonly TelegramBotClient Bot = new TelegramBotClient(Consts.TelegramBotAccessToken);

		static string VkAccessToken;

		static void Main(string[] args)
		{
			var me = Bot.GetMeAsync().Result;
			Console.Title = me.Username;

			Bot.OnMessage += BotOnMessageReceived;

			Bot.StartReceiving(Array.Empty<UpdateType>());
			Console.WriteLine($"Start listening for @{me.Username}");
			Console.ReadLine();
			Console.ReadLine();
			Bot.StopReceiving();
		}

		private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
		{
			var message = messageEventArgs.Message;

			if (message == null || message.Type != MessageType.Text)
				return;
			switch (message.Text.Split(' ').First())
			{
				case "/auth":
					const string getVkCodeUrl = @"https://oauth.vk.com/authorize?client_id=" + Consts.VkAppId + "&display=page&redirect_uri=https://oauth.vk.com/blank.html&display=page&scope=messages,offline&response_type=code";

					await Bot.SendTextMessageAsync(message.Chat.Id, getVkCodeUrl);

					break;
				case "/setToken=":
					string vkTokenUrl = @"https://oauth.vk.com/access_token?client_id=" + Consts.VkAppId + "&client_secret=" + Consts.VkAppSecureKey + "&redirect_uri=https://oauth.vk.com/blank.html&code=";
					string vkCode = message.Text.Replace("/setToken= ", "");
					vkTokenUrl += vkCode;

					var vkResult = Client.GetStringAsync(vkTokenUrl);
					var jObject = JObject.Parse(vkResult.Result);
					VkAccessToken = (string)jObject.SelectToken("access_token");
					await Bot.SendTextMessageAsync(message.Chat.Id, VkAccessToken);

					break;
			}
		}
	}
}
