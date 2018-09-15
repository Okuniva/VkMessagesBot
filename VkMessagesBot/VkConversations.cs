namespace VkConversations
{
	using System;
	using System.Collections.Generic;

	using System.Globalization;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Converters;

	public partial class Conversations
	{
		[JsonProperty("response")]
		public Response Response { get; set; }
	}

	public partial class Response
	{
		[JsonProperty("count")]
		public long Count { get; set; }

		[JsonProperty("items")]
		public List<Item> Items { get; set; }

		[JsonProperty("unread_count")]
		public long UnreadCount { get; set; }
	}

	public partial class Item
	{
		[JsonProperty("conversation")]
		public Conversation Conversation { get; set; }

		[JsonProperty("last_message")]
		public LastMessage LastMessage { get; set; }
	}

	public partial class Conversation
	{
		[JsonProperty("peer")]
		public Peer Peer { get; set; }

		[JsonProperty("in_read")]
		public long InRead { get; set; }

		[JsonProperty("out_read")]
		public long OutRead { get; set; }

		[JsonProperty("last_message_id")]
		public long LastMessageId { get; set; }

		[JsonProperty("unread_count")]
		public long UnreadCount { get; set; }

		[JsonProperty("can_write")]
		public CanWrite CanWrite { get; set; }
	}

	public partial class CanWrite
	{
		[JsonProperty("allowed")]
		public bool Allowed { get; set; }
	}

	public partial class Peer
	{
		[JsonProperty("id")]
		public long Id { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("local_id")]
		public long LocalId { get; set; }
	}

	public partial class LastMessage
	{
		[JsonProperty("date")]
		public long Date { get; set; }

		[JsonProperty("from_id")]
		public long FromId { get; set; }

		[JsonProperty("id")]
		public long Id { get; set; }

		[JsonProperty("out")]
		public long Out { get; set; }

		[JsonProperty("peer_id")]
		public long PeerId { get; set; }

		[JsonProperty("text")]
		public string Text { get; set; }

		[JsonProperty("conversation_message_id")]
		public long ConversationMessageId { get; set; }

		[JsonProperty("fwd_messages")]
		public List<object> FwdMessages { get; set; }

		[JsonProperty("important")]
		public bool Important { get; set; }

		[JsonProperty("random_id")]
		public long RandomId { get; set; }

		[JsonProperty("attachments")]
		public List<object> Attachments { get; set; }

		[JsonProperty("is_hidden")]
		public bool IsHidden { get; set; }
	}

	public partial class Conversations
	{
		public static Conversations FromJson(string json) => JsonConvert.DeserializeObject<Conversations>(json, VkConversations.Converter.Settings);
	}

	public static class Serialize
	{
		public static string ToJson(this Conversations self) => JsonConvert.SerializeObject(self, VkConversations.Converter.Settings);
	}

	internal static class Converter
	{
		public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
		{
			MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
			DateParseHandling = DateParseHandling.None,
			Converters = {
				new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
			},
		};
	}
}
