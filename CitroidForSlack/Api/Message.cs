namespace CitroidForSlack
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

		public Message Roid(ICitroid roid)
		{
			citroid = roid;
			return this;
		}

		public 
	}

}