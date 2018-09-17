using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using VkConversations;
using VkMessagesHistory;
using VkUsers;

namespace VkMessagesBot
{
	static class Program
	{
		static readonly HttpClient Client = new HttpClient();

		static readonly TelegramBotClient Bot = new TelegramBotClient(Consts.TelegramBotAccessToken);
		
		static bool VkConnect = true;

		static bool EnableGetMessages = true;

		static void Main(string[] args)
		{
			var me = Bot.GetMeAsync().Result;
			Console.Title = me.Username;

			Bot.OnMessage += BotOnMessageReceived;
			Bot.OnCallbackQuery += BotOnCallbackQueryReceived;

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
					const string getVkCodeUrl = @"https://oauth.vk.com/authorize?client_id=" + Consts.VkAppId +
					                            @"&display=page&redirect_uri=https://oauth.vk.com/blank.html&display=page&scope=messages,offline&response_type=code";

					await Bot.SendTextMessageAsync(message.Chat.Id, getVkCodeUrl);

					break;
				case "/setToken=":
					var vkTokenUrl = @"https://oauth.vk.com/access_token?client_id=" + Consts.VkAppId +
					                 @"&client_secret=" + Consts.VkAppSecureKey +
					                 @"&redirect_uri=https://oauth.vk.com/blank.html&code=";

					var vkCode = message.Text.Replace("/setToken= ", "");
					vkTokenUrl += vkCode;

					var vkResult = await Client.GetStringAsync(vkTokenUrl);
					var jObject = JObject.Parse(vkResult);

					Consts.VkAccessToken = (string)jObject.SelectToken("access_token");

					await Bot.SendTextMessageAsync(message.Chat.Id, Consts.VkAccessToken);

					break;
				case "/getmessages":
					if(!EnableGetMessages)
						return;

					EnableGetMessages = false;
					VkConnect = true;
					while (VkConnect)
					{
						var conversationsUrl =
							@"https://api.vk.com/method/messages.getConversations?v=5.80&access_token=" +
							Consts.VkAccessToken +
							@"&offset=0&count=200&extended=0&filter=unread";

						var conversationsJson = await Client.GetStringAsync(conversationsUrl);
						var conversations = Conversations.FromJson(JObject.Parse(conversationsJson).ToString());

						foreach (var conversation in conversations.Response.Items)
						{
							var historyUrl =
								@"https://api.vk.com/method/messages.getHistory?v=5.80&access_token=" + Consts.VkAccessToken +
								"&offset=0&user_id=" + conversation.Conversation.Peer.Id + "&count=" +
								conversation.Conversation.UnreadCount;

							var historyJson = await Client.GetStringAsync(historyUrl);
							var unreadMessages = History.FromJson(JObject.Parse(historyJson).ToString());
							var messageList = new List<MessageObject>();
							var i = 0;
							foreach (var unreadMessage in unreadMessages.Response.Items)
							{
								var messageObj = new MessageObject
								{
									Id = i,
									Text = unreadMessage.Text,
									FromId = unreadMessage.FromId
								};
								messageList.Add(messageObj);
								i++;
							}

							foreach (var messageObj in messageList)
							{
								var userUrl =
									@"https://api.vk.com/method/users.get?v=5.80&access_token=" +
									Consts.VkAccessToken +
									@"&user_ids=" + messageObj.FromId;
								var userJson = await Client.GetStringAsync(userUrl);
								var user = Users.FromJson(userJson);


								
								await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
								await Task.Delay(500); // simulate longer running task
								var inlineKeyboard = new InlineKeyboardMarkup(new[]
								{
									new [] // first row
									{
										InlineKeyboardButton.WithCallbackData("Send"),
									}
								});

								await Bot.SendTextMessageAsync(message.Chat.Id, "от: " + user.Response[0].FirstName + " "
								                                                + user.Response[0].LastName + " текст: "
								                                                + messageObj.Text, replyMarkup: inlineKeyboard);
							}
						}

						Thread.Sleep(300000);
					}

					break;
				case "/send":
					var sendUrl = @"https://api.vk.com/method/messages.send?v=5.80&access_token=" +
					              Consts.VkAccessToken +
					              @"&user_id=290761122" + @"&random_id=" + new DateTime().Millisecond +
					              @"&message=" + message.Text.Replace("/send ", "");
					await Client.GetStringAsync(sendUrl);
					break;
				case "/stop":
					VkConnect = false;
					EnableGetMessages = true;
					break;;
			}
		}

		private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
		{
			var callbackQuery = callbackQueryEventArgs.CallbackQuery;

			await Bot.SendTextMessageAsync(
				callbackQuery.Message.Chat.Id,
				$"Received send click");
		}
	}
}
