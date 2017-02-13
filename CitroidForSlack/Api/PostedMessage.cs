using Newtonsoft.Json.Linq;

namespace CitroidForSlack
{
	public class PostedMessage
	{
		public bool ok { get; set; }
		public string ts { get; set; }
		public string channel { get; set; }
		public JObject message { get; set; }
	}

}