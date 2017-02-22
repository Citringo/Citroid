using CitroidForSlack.Plugins.Utiline;
using CitroidForSlack.Plugins.Utiline.Api;

namespace CitroidForSlack.Utiline.Commands
{
	public class CommandUnalias : ICommand
	{
		private readonly MessageBotUtiline _bot;

		public string Name => "unalias";

		public string Usage => $@"登録されたエイリアスを削除します。
{Name}: <name>...";

		public string Process(string[] args)
		{
			var count = 0;
			foreach (var name in args)
				count += _bot.Aliases.RemoveAll(a => a.Name == name);
			_bot.ReloadCommand();
			return count > 0 ? $"合計 {count} 個のエイリアスを削除しました。" : "削除されるものはありませんでした。";
		}

		public CommandUnalias(MessageBotUtiline bot) => _bot = bot;
	}
}
