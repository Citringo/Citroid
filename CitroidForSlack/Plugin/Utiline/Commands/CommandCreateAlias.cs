using CitroidForSlack.Plugins.Utiline;
using CitroidForSlack.Plugins.Utiline.Api;
using CitroidForSlack.Plugins.Utiline.Exceptions;
using System.Linq;

namespace CitroidForSlack.Utiline.Commands
{
	public class CommandCreateAlias : ICommand
	{
		private MessageBotUtiline _bot;

		public CommandCreateAlias(MessageBotUtiline bot)
		{
			_bot = bot;
		}

		public string Name => "alias";

		public string Usage => $@"コマンドと引数のセットに名前をつけて、エイリアスという新しいコマンドを作成します。
{Name}: <name>, <cmdname>, [args]...
name: エイリアスにつける名前です。この名前を使って、コマンドを呼び出します。
cmdname: コマンドの名前です。エイリアスを指定することもできます。
";

		public string Process(string[] args)
		{
			if (args.Length == 1)
				throw new WrongUsageException();
			else if (args.Length == 0)
			{
				return string.Join("\n", _bot.Aliases.Select(a => $"{a.Name} = {a.CommandName}: {string.Join(", ", a.Arguments)}"));
			}
			var name = args[0];
			var cmdname = args[1];
			string[] arguments = args.Skip(2).ToArray();
			_bot.Aliases.RemoveAll(p => p.Name == name);
			_bot.Aliases.Add(new Alias(name, cmdname, arguments));
			_bot.ReloadCommand();
			return "追加しました。";
		}
	}
}
