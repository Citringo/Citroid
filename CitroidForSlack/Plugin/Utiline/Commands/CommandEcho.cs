using NCalc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CitroidForSlack
{
	public class CommandEcho : ICommand
	{
		public string Name => "echo";

		public string Usage => $@"指定した文字列を発言します。
{Name}: <message>";

		public string Process(string[] args) => string.Join(",", args);
	}

	public class CommandCalc : ICommand
	{
		public string Name => "calc";

		public string Usage => $@"電卓です。一般的な計算式や、一部の関数を使えます。
{Name}: <message>";

		public string Process(string[] args)
		{
			var expr = string.Join(",", args);
			try
			{
				return new Expression(expr).Evaluate().ToString();
			}
			catch (EvaluationException ex)
			{
				return "Calc Error: " + ex.Message;
			}
		}
	}



	public class CommandMakeBot : ICommand
	{
		public const string MODE_PERFECT = "perfect";
		public const string MODE_PARTIAL = "partial";
		public const string MODE_REGEX = "regex";
		private MessageBotUtiline parent;

		public CommandMakeBot(MessageBotUtiline bot)
		{
			parent = bot;
		}

		public string Name => "mkbot";

		public string Usage => $@" Botの返事機能を作るコマンドです。
{Name}: <pattern>, <reply>, [mode], [name], [emoji]

pattern: 一致条件として指定するパターン。mode 引数が regex の場合、正規表現を使って指定します。 例: ""(.+?)を?ください""
  reply: 返事のテキストです。 mode 引数が regex の場合、.NET の置換表現を使えます。 例: ""$1 はないです。""
   mode: 挙動を指定します。 モードには次の項目を指定でき、どれでもない場合はエラーを返します。
         {MODE_PERFECT}: 完全一致モード。
         {MODE_PARTIAL}: 部分一致モード。
         {MODE_REGEX}: 正規表現モード。";
		
		public string Process(string[] args)
		{
			if (args.Length < 2 || args.Length > 5)
				throw new WrongUsageException();
			var mode = args.Length > 2 ? args[2] : MODE_PARTIAL;
			mode = mode.ToLower().Trim();
			var name = args.Length > 3 ? args[3] : "";
			var emoji = args.Length > 4 ? args[4] : "";

			var pattern = args[0];
			var reply = args[1];
			switch (mode)
			{
				case MODE_PERFECT:
					parent.Replies.Add(new UtilineReplyPerfect(pattern, reply, name, emoji));
					break;
				case MODE_PARTIAL:
					parent.Replies.Add(new UtilineReplyPartial(pattern, reply, name, emoji));
					break;
				case MODE_REGEX:
					parent.Replies.Add(new UtilineReplyRegex(pattern, reply, name, emoji));
					break;
				default:
					throw new IllegalCommandCallException($"モードに {mode} は指定できません。");
			}
			return "登録しました。";

		}

		

	}

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

	/// <summary>
	/// エイリアスをコマンドインターフェイスに対応させるためのラッパー クラスです。
	/// </summary>
	public class AliasAsCommand : ICommand
	{
		private readonly MessageBotUtiline _bot;



		public Alias Alias { get; }

		string ICommand.Usage => "エイリアス";

		string ICommand.Name => Alias.Name;


		public AliasAsCommand(MessageBotUtiline bot, Alias a)
		{
			_bot = bot;
			Alias = a;
		}

		public string Process(string[] args)
		{
			var a = Alias.Arguments.ToArray();
			for (var i = 0; i < a.Length; i++)
			{
				a[i] = a[i]
					.Replace("$0", Alias.CommandName)
					.Replace("$#", args.Length.ToString())
					.Replace("$@", string.Join(", ", Alias.Arguments))
					.Replace("$*", string.Join(", ", Alias.Arguments));
				for (var j = args.Length; j > 0; j--)
					a[i] = a[i].Replace("$" + j, args[j - 1]);
			}

			ICommand cmd = _bot.GetCommand(Alias.CommandName);
			if (cmd == null)
				throw new IllegalCommandCallException($"{Alias.CommandName}: そんなコマンドはありません。");

			return cmd.Process(a);
		}
		
	}

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

	public class CommandListBot : ICommand
	{
		private readonly MessageBotUtiline _bot;

		public string Name => "lsbot";

		public string Usage => $@"返事のリストを表示します。引数はありません。";

		public string Process(string[] args)
		{
			var sb = new StringBuilder();
			foreach (UtilineReplyBase urb in _bot.Replies)
			{
				sb.AppendLine($"{urb.ToString()}");
			}
			if (string.IsNullOrEmpty(sb.ToString()))
				sb.Append("返事は登録されていません。");
			return sb.ToString();
		}

		public CommandListBot(MessageBotUtiline bot) => _bot = bot;
	}

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
