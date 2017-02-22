using System.Collections.Specialized;
using System.Threading.Tasks;

namespace CitroidForSlack.Api
{

	public class Message
	{
		public string type { get; set; }
		public string channel { get; set; }
		public string user { get; set; }
		public string text { get; set; }
		public string ts { get; set; }
		public string team { get; set; }
		public string subtype { get; set; }
		public string bot_id { get; set; }
		public string username { get; set; }

		private ICitroid citroid;

		internal Message Roid(ICitroid roid)
		{
			citroid = roid;
			return this;
		}

		public async Task AddReactionAsync(string emoji)
		{
			await citroid.RequestAsync("reactions.add", new NameValueCollection
			{
				{ "name", emoji },
				{"timestamp", ts },
				{"channel", channel }
			});
		}

		public async Task RemoveReactionAsync(string emoji)
		{
			await citroid.RequestAsync("reactions.remove", new NameValueCollection
			{
				{ "name", emoji },
				{"timestamp", ts },
				{"channel", channel }
			});
		}
	}

}