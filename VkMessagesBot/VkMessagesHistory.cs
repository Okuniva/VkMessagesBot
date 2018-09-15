namespace VkMessagesHistory
{
	using System;
	using System.Collections.Generic;

	using System.Globalization;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Converters;

	public partial class History
	{
		[JsonProperty("response")]
		public Response Response { get; set; }
	}

	public partial class Response
	{
		[JsonProperty("count")]
		public int Count { get; set; }

		[JsonProperty("items")]
		public List<Item> Items { get; set; }
	}

	public partial class Item
	{
		[JsonProperty("date")]
		public string Date { get; set; }

		[JsonProperty("from_id")]
		public string FromId { get; set; }

		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("out")]
		public string Out { get; set; }

		[JsonProperty("peer_id")]
		public string PeerId { get; set; }

		[JsonProperty("text")]
		public string Text { get; set; }

		[JsonProperty("conversation_message_id")]
		public string ConversationMessageId { get; set; }

		[JsonProperty("fwd_messages")]
		public List<object> FwdMessages { get; set; }

		[JsonProperty("important")]
		public bool Important { get; set; }

		[JsonProperty("random_id")]
		public string RandomId { get; set; }

		[JsonProperty("attachments")]
		public List<object> Attachments { get; set; }

		[JsonProperty("is_hidden")]
		public bool IsHidden { get; set; }
	}

	public partial class History
	{
		public static History FromJson(string json) => JsonConvert.DeserializeObject<History>(json, VkHistory.Converter.Settings);
	}

	public static class Serialize
	{
		public static string ToJson(this History self) => JsonConvert.SerializeObject(self, VkHistory.Converter.Settings);
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
