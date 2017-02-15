using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System;

namespace CitroidForSlack
{
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

		public PostedMessage Roid(ICitroid citroid)
		{
			Citroid = citroid;
			return this;
		}
	}
}