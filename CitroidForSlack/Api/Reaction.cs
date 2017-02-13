using Newtonsoft.Json.Linq;

namespace CitroidForSlack
{
	public class Reaction
	{
		public string type { get; set; }
		public string user { get; set; }
		public string reaction { get; set; }
		public string item_user { get; set; }
		public JObject item { get; set; }
		public string event_ts { get; set; }
	}

}