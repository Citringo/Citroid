using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System;

namespace CitroidForSlack
{
	public delegate void MessageReactionEventHandler(string emoji, string user);

	public class PostedMessage
	{
		public bool ok { get; set; }
		public string ts { get; set; }
		public string channel { get; set; }
		public JObject message { get; set; }
		public ICitroid Citroid { get; set; }

		/// <summary>
		/// このメッセージを編集します。
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public async Task<PostedMessage> UpdateAsync(string text)
		{
			return (await Citroid.RequestAsync("chat.update", new NameValueCollection
			{
				{ "ts", ts },
				{ "channel", channel },
				{"text", text }
			})).ToObject<PostedMessage>().Roid(Citroid);
		}

		public event MessageReactionEventHandler ReactionAdded;
		public event MessageReactionEventHandler ReactionRemoved;

		public PostedMessage Roid(ICitroid citroid)
		{
			Citroid = citroid;
			citroid.ReactionAdded += (sender, e) =>
			{
				if (e.item.TryGetValue("ts", out var ts) && ts.Type == JTokenType.String && ts.Value<string>() == this.ts)
					ReactionAdded?.Invoke(e.reaction, e.user);
			};

			citroid.ReactionRemoved += (sender, e) =>
			{
				if (e.item.TryGetValue("ts", out var ts) && ts.Type == JTokenType.String && ts.Value<string>() == this.ts)
					ReactionRemoved?.Invoke(e.reaction, e.user);
			};
			return this;
		}
	}
}