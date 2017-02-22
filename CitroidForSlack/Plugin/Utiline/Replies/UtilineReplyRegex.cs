using System.Text.RegularExpressions;

namespace CitroidForSlack.Plugin.Utiline.Replies
{
	class UtilineReplyRegex : UtilineReplyBase
	{
		public UtilineReplyRegex(string pattern, string reply, string name = "", string emoji = "") : base(pattern, reply, name, emoji) { }

		public override bool Match(string input) => Regex.IsMatch(input, _pattern);

		public override string Reply(string input) => Regex.Match(input, _pattern).Result(_reply);

		public override string ToString() => "正規表現一致: " + base.ToString();
	}
}
