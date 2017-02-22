namespace CitroidForSlack.Plugin.Utiline.Replies
{
	class UtilineReplyPerfect : UtilineReplyBase
	{
		public UtilineReplyPerfect(string pattern, string reply, string name = "", string emoji = "") : base(pattern, reply, name, emoji) { }

		public override bool Match(string input) => input == _pattern;

		public override string Reply(string input) => _reply;

		public override string ToString() => "完全一致: " + base.ToString();
	}
}
