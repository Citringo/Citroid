using CitroidForSlack.Api;
using CitroidForSlack.Plugin.Utiline.Replies;
using CitroidForSlack.Plugins.Utiline.Api;
using CitroidForSlack.Plugins.Utiline.Exceptions;
using CitroidForSlack.Utiline.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CitroidForSlack.Plugins.Utiline
{

	public class MessageBotUtiline : IMessageBot
	{
		public string Help => "Utility Bot.";

		public string Name => "Utiline";

		public string Copyright => "(C)2017 Citrine";

		public string Version => "1.1.0";

		public bool CanExecute(Message mes) => true;


		public List<ICommand> Commands { get; } = new List<ICommand>();

		public List<UtilineReplyBase> Replies { get; } = new List<UtilineReplyBase>();

		public List<Alias> Aliases { get; private set; } = new List<Alias>();

		public ICommand GetCommand(string cmd) => Commands.FirstOrDefault(c => c.Name == cmd);

		public Task InitializeAsync(ICitroid citroid)
		{
			LoadAliases();

			LoadCommand();

			LoadReply();

			return Task.Delay(0);
		}

		private void LoadAliases()
		{
			if (!File.Exists(ALIAS_FILE))
				return;

			Aliases = JsonConvert.DeserializeObject<List<Alias>>(File.ReadAllText(ALIAS_FILE));
		}

		private void SaveAliases()
		{
			File.WriteAllText(ALIAS_FILE, JsonConvert.SerializeObject(Aliases));
		}

		private void LoadCommand()
		{
			Commands.AddRange(new ICommand[] {
				new CommandEcho(),
				new CommandCalc(),
				new CommandHelp(this),
				new CommandMakeBot(this),
				new CommandCreateAlias(this),
				new CommandListBot(this),
				new CommandRemoveBot(this),
				new CommandPipe(this),
				new CommandUnalias(this),
			});

			Commands.AddRange(Aliases.Select(a => new AliasAsCommand(this, a)));
		}

		public void ReloadCommand()
		{
			Commands.Clear();
			LoadCommand();
		}

		

		public void Exit(ICitroid citroid)
		{
			SaveReply();
			SaveAliases();
		}


		enum ReplyMode
		{
			Perfect,
			Partial,
			Regex
		}

		class SerializableReply
		{
			public ReplyMode Mode { get; set; }
			public string Pattern { get; set; }
			public string Replacement { get; set; }
			public string Name { get; set; }
			public string Emoji { get; set; }

		}

		const string REPLY_FILE = "utiline-reply.json";
		const string ALIAS_FILE = "utiline-alias.json";
		private void LoadReply()
		{
			if (!File.Exists(REPLY_FILE))
				return;
			foreach (SerializableReply reply in JsonConvert.DeserializeObject<List<SerializableReply>>(File.ReadAllText(REPLY_FILE)))
			{
				switch (reply.Mode)
				{
					case ReplyMode.Perfect:
						Replies.Add(new UtilineReplyPerfect(reply.Pattern, reply.Replacement, reply.Name, reply.Emoji));
						break;
					case ReplyMode.Partial:
						Replies.Add(new UtilineReplyPartial(reply.Pattern, reply.Replacement, reply.Name, reply.Emoji));
						break;
					case ReplyMode.Regex:
						Replies.Add(new UtilineReplyRegex(reply.Pattern, reply.Replacement, reply.Name, reply.Emoji));
						break;
					default:
						Debug.WriteLine($"Unexpected reply mode\"{reply.Mode}\". Ignore it.");
						break;
				}
			}
		}

		private void SaveReply()
		{
			var jsonbase = new List<SerializableReply>();
			foreach (UtilineReplyBase reply in Replies)
			{
				ReplyMode rm;
				switch (reply)
				{
					case UtilineReplyPartial t:
						rm = ReplyMode.Partial;
						break;
					case UtilineReplyPerfect t:
						rm = ReplyMode.Perfect;
						break;
					case UtilineReplyRegex t:
						rm = ReplyMode.Regex;
						break;
					default:
						throw new InvalidOperationException("予期されない型。");
				}
				jsonbase.Add(new SerializableReply {Mode = rm, Pattern = reply.Pattern, Replacement = reply.Replacement, Name = reply.UserName, Emoji = reply.UserEmoji});
			}
			File.WriteAllText(REPLY_FILE, JsonConvert.SerializeObject(jsonbase));
		}

		

		public enum Token
		{
			Name,
			Coron,
			Args,
		}

		class CommandNode
		{
			public string Cmd { get; }
			public string[] Args { get; }
			public CommandNode(string cmd, string[] args)
			{
				Cmd = cmd; Args = args;
			}
		}

		static CommandNode ParseCommand(string cs, string user = "")
		{
			if (string.IsNullOrEmpty(cs))
				return null;

			string name = null;
			List<string> args = null;

			var buffer = "";
			var quotFlag = false;
			Token token = Token.Name;
			buffer = "";
			name = "";
			args = new List<string>();
			for (var i = 0; i < cs.Length; i++)
			{
				var c = cs[i];
				var cm1 = i > 0 ? cs[i - 1] : '\0';
				var cp1 = i < cs.Length - 1 ? cs[i + 1] : '\0';
				if ((!quotFlag) && (char.IsControl(c) || char.IsSeparator(c) || char.IsWhiteSpace(c)))
					continue;
				switch (token)
				{
					case Token.Name:
						name += c;
						switch (cp1)
						{
							case ':':
								token = Token.Coron;
								break;
						}
						break;
					case Token.Coron:
						
						token = Token.Args;
						break;
					case Token.Args:

						switch (c)
						{
							case '\\':
								switch (cp1)
								{
									case 'n':
										buffer = buffer + '\n';
										break;
									case '"':
										buffer = buffer + '"';
										break;
									case '\\':
										buffer = buffer + '\\';
										break;
									case 'u':
										buffer = buffer + user;
										break;
									default:
										throw new ParseCommandException($@"Script Error: Invalid escape sequence \{cp1}");
								}
								i++;
								break;
							case '"':
								quotFlag = !quotFlag;
								break;
							default:

								if ((c == ',') && !quotFlag)
								{
									args?.Add(buffer);
									buffer = "";
								}
								else
								{
									buffer = buffer + c;
								}


								break;
						}
						break;
					default:
						throw new ParseCommandException($@"Internal Error: Unknown token ""{token}""");
				}
			}
			if (!string.IsNullOrEmpty(buffer))
				args.Add(buffer);
			if (quotFlag)
				throw new ParseCommandException("Script Error: Unmatched Double Quotation");
			return new CommandNode(name, args.ToArray());
		}


		public async Task RunAsync(Message mes, ICitroid citroid)
		{
			if (!await ProcessCommand(mes, citroid))
				await ProcessReply(mes, citroid);
		}

		private async Task<bool> ProcessCommand(Message mes, ICitroid citroid)
		{
			CommandNode node;
			try
			{
				node = ParseCommand(mes.text, citroid.GetUser(mes.user));
			}
			catch (ParseCommandException)
			{
				// すべての発言をパースするので、この場合はコマンドではないと判断し何もしない。
				return false;
			}
			if (node == null)
				return false;

			ICommand cmd = GetCommand(node.Cmd);

			if (cmd == null)
				return false;

			try
			{
				await citroid.PostAsync(mes.channel, cmd.Process(node.Args));
			}
			catch (WrongUsageException)
			{
				await citroid.PostAsync(mes.channel, $"{cmd.Usage}");
			}
			catch (IllegalCommandCallException ex)
			{
				await citroid.PostAsync(mes.channel, $"Err: *{ex.Message}*");
			}
			catch (Exception ex)
			{
				await citroid.PostAsync(mes.channel, $"コマンド内部でエラー( *{ex.GetType().Name}* ): *{ex.Message}*");
			}
			return true;
		}

		private async Task ProcessReply(Message mes, ICitroid citroid)
		{
			if (mes.subtype == "bot_message")
				return;
			foreach (UtilineReplyBase r in Replies)
			{
				if (r.Match(mes.text))
					await citroid.PostAsync(mes.channel, r.Reply(mes.text), r.UserName ?? "", "", r.UserEmoji ?? "");
			}
		}
	}
}
