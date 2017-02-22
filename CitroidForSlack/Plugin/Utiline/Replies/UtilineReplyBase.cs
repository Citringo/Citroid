using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitroidForSlack.Plugin.Utiline.Replies
{
	public abstract class UtilineReplyBase
	{
		protected string _pattern;
		protected string _reply;
		public string Pattern => _pattern;
		public string Replacement => _reply;
		public string UserName { get; }
		public string UserEmoji { get; }
		public abstract bool Match(string input);
		public UtilineReplyBase(string pattern, string reply, string name = "", string emoji = "")
		{
			_pattern = pattern;
			_reply = reply;
			UserName = name;
			UserEmoji = emoji;
		}
		public abstract string Reply(string input);

		public override string ToString() => $"{Pattern} => {Replacement}";
	}
}
