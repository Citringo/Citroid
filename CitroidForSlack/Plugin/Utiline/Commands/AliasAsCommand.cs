using CitroidForSlack.Plugins.Utiline;
using CitroidForSlack.Plugins.Utiline.Api;
using CitroidForSlack.Plugins.Utiline.Exceptions;
using System.Linq;

namespace CitroidForSlack.Utiline.Commands
{
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
}
