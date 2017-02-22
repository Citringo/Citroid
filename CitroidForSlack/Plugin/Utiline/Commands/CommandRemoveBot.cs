using CitroidForSlack.Plugins.Utiline;
using CitroidForSlack.Plugins.Utiline.Api;
using CitroidForSlack.Plugins.Utiline.Exceptions;

namespace CitroidForSlack.Utiline.Commands
{
	public class CommandRemoveBot : ICommand
	{
		private readonly MessageBotUtiline _bot;

		public string Name => "rmbot";

		public string Usage => $@"返事を削除します。
{Name}: [pattern]

pattern: パターンを指定することで、その返事を削除します。省略した場合、すべての返事を削除します。";

		public string Process(string[] args)
		{
			if (args.Length > 1)
				throw new WrongUsageException();
			string pattern = null;
			if (args.Length == 1)
				pattern = args[0];
			if (pattern == null)
			{
				_bot.Replies.Clear();
				return "全消去しました。";
			}
			else
			{
				var count = _bot.Replies.RemoveAll(u => u.Pattern == pattern);
				return count > 0 ? $"{count} 件の返事を削除しました。" : "削除する返事はありませんでした。";
			}

		}

		public CommandRemoveBot(MessageBotUtiline bot) => _bot = bot;
	}
}
