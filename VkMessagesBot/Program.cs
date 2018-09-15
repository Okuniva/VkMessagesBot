using System;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using VkConversations;
using VkMessagesHistory;

namespace VkMessagesBot
{
	static class Program
	{
		static readonly HttpClient Client = new HttpClient();

		static readonly TelegramBotClient Bot = new TelegramBotClient(Consts.TelegramBotAccessToken);

		private static string VkAccessToken = "c9bf47ca401eb679e07e8a23fce6d7ae5393aa81c65080ab139d2f255a19d0c821fd03d57c70649bd5a38";

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
					const string getVkCodeUrl = @"https://oauth.vk.com/authorize?client_id=" + Consts.VkAppId + @"&display=page&redirect_uri=https://oauth.vk.com/blank.html&display=page&scope=messages,offline&response_type=code";

					await Bot.SendTextMessageAsync(message.Chat.Id, getVkCodeUrl);

					break;
				case "/setToken=":
					var vkTokenUrl = @"https://oauth.vk.com/access_token?client_id=" + Consts.VkAppId + @"&client_secret=" + Consts.VkAppSecureKey + @"&redirect_uri=https://oauth.vk.com/blank.html&code=";
					var vkCode = message.Text.Replace("/setToken= ", "");
					vkTokenUrl += vkCode;

					var vkResult = await Client.GetStringAsync(vkTokenUrl);
					var jObject = JObject.Parse(vkResult);
					VkAccessToken = (string)jObject.SelectToken("access_token");
					await Bot.SendTextMessageAsync(message.Chat.Id, VkAccessToken);

					break;
				case "/getmessages":
					var conversationsUrl =
						@"https://api.vk.com/method/messages.getConversations?v=5.80&access_token=" + VkAccessToken +
						@"&offset=0&count=200&extended=0&filter=unread";
					
					var conversationsJson = await Client.GetStringAsync(conversationsUrl);
					var conversations = Conversations.FromJson(JObject.Parse(conversationsJson).ToString());

					foreach (var conversation in conversations.Response.Items)
					{
						var historyUrl =
							@"https://api.vk.com/method/messages.getHistory?v=5.80&access_token=" + VkAccessToken +
							"&offset=0&user_id=" + conversation.Conversation.Peer.Id + "&count=" + conversation.Conversation.UnreadCount;

						var historyJson = await Client.GetStringAsync(historyUrl);
						var unreadMessages = History.FromJson(JObject.Parse(historyJson).ToString());
						foreach (var unreadMessage in unreadMessages.Response.Items)
						{
							await Bot.SendTextMessageAsync(message.Chat.Id, unreadMessage.Text);
						}
					}


					break;
			}
		}
	}
}
