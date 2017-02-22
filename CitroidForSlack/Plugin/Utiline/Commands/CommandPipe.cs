using CitroidForSlack.Plugins.Utiline;
using CitroidForSlack.Plugins.Utiline.Api;
using CitroidForSlack.Plugins.Utiline.Exceptions;
using System.Linq;

namespace CitroidForSlack.Utiline.Commands
{
	public class CommandPipe : ICommand
	{
		private readonly MessageBotUtiline _bot;

		public string Name => "pipe";

		public string Usage => $@"コマンドを連結して一つのコマンドにします。
{Name}: <cmd>|<cmd>|...|<cmd>, [args]...
第一引数で、コマンド名をパイプで繋ぎます。
第二引数で、最初のコマンドに与える引数を指定します。
2個目のコマンド以降には、第一引数に前のコマンドの出力が入ります。";

		public string Process(string[] args)
		{
			if (args.Length < 2)
				throw new WrongUsageException();
			string[] cmds = args[0].Split('|');
			args = args.Skip(1).ToArray();
			var topCmd = cmds[0];
			string[] followingCmds = cmds.Skip(1).ToArray();
			var output = _bot.GetCommand(topCmd).Process(args);
			foreach (var name in followingCmds)
				output = _bot.GetCommand(name).Process(new[] { output });
			return output;
		}

		public CommandPipe(MessageBotUtiline utiline) => _bot = utiline;
	}
}
