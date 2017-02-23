using CitroidForSlack.Plugin.Utiline.Replies;
using CitroidForSlack.Plugins.Utiline;
using CitroidForSlack.Plugins.Utiline.Api;
using System.Linq;
using System.Text;

namespace CitroidForSlack.Utiline.Commands
{
	public class CommandListBot : ICommand
	{
		private readonly MessageBotUtiline _bot;

		public string Name => "lsbot";

		public string Usage => $@"返事のリストを表示します。引数はありません。";

		public string Process(string[] args)
		{
			var sb = new StringBuilder();
			foreach (UtilineReplyBase urb in _bot.Replies.ToList())
			{
				sb.AppendLine($"{urb.ToString()}");
			}
			if (string.IsNullOrEmpty(sb.ToString()))
				sb.Append("返事は登録されていません。");
			return sb.ToString();
		}

		public CommandListBot(MessageBotUtiline bot) => _bot = bot;
	}
}
