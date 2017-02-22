using CitroidForSlack.Extensions;
using CitroidForSlack.Plugins.Utiline;
using CitroidForSlack.Plugins.Utiline.Api;
using System.Linq;
using System.Text;

namespace CitroidForSlack.Utiline.Commands
{
	public class CommandHelp : ICommand
	{
		private readonly MessageBotUtiline _bot;

		public string Name => "help";

		public string Usage => $@"ヘルプを表示します。
{Name}: [command]
command: コマンド名を指定した場合、そのコマンドの詳しいヘルプを表示します。";

		public string Process(string[] args)
		{
			if (args.Length == 0)
			{
				var sb = new StringBuilder();
				sb.AppendLine($@"{_bot.Name} version{_bot.Version}
便利なコマンドと、拡張可能な返事機能を備えるBotです。
コマンドを呼び出すには、次のように入力します。 <コマンド名>:<引数>

コマンド名: 説明");
				foreach (ICommand cmd in _bot.Commands.OrderBy(c => c.Name))
				{
					sb.AppendLine($"{cmd.Name}: {cmd.Usage.SplitWithNewLine()[0]}");
				}
				sb.AppendLine($"より詳しい説明を読むには、{Name}: <コマンド名> を入力します。");
				return sb.ToString();
			}
			return _bot.Commands.FirstOrDefault(c => c.Name == args[0].Trim())?.Usage ?? "そんなコマンドはありません。 help: と入力して、完全なヘルプを参照してください。";
		}

		public CommandHelp(MessageBotUtiline bot) => _bot = bot;
	}
}
