using CitroidForSlack.Plugin.Utiline.Replies;
using CitroidForSlack.Plugins.Utiline;
using CitroidForSlack.Plugins.Utiline.Api;
using CitroidForSlack.Plugins.Utiline.Exceptions;

namespace CitroidForSlack.Utiline.Commands
{
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
}
