namespace CitroidForSlack.Plugin.Utiline.Replies
{
	class UtilineReplyPartial : UtilineReplyBase
	{
		public UtilineReplyPartial(string pattern, string reply, string name = "", string emoji = "") : base(pattern, reply, name, emoji) { }

		public override bool Match(string input) => input.Contains(_pattern);

		public override string Reply(string input) => _reply;

		public override string ToString() => "部分一致: " + base.ToString();
	}
}
