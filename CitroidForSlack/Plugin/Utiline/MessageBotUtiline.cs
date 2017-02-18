using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CitroidForSlack
{


	[Serializable]
	public class ParseCommandException : Exception
	{
		public ParseCommandException() { }
		public ParseCommandException(string message) : base(message) { }
		public ParseCommandException(string message, Exception inner) : base(message, inner) { }
		protected ParseCommandException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}


	public class MessageBotUtiline : IMessageBot
	{
		public string Help => "Utility Bot.";

		public string Name => "Utiline";

		public string Copyright => "(C)2017 Citrine";

		public string Version => "1.0.0pre-alpha";

		public bool CanExecute(Message mes) => true;

		public void Exit(ICitroid citroid) { }

		public List<ICommand> Commands { get; } = new List<ICommand>();

		public ICommand GetCommand(string cmd) => Commands.FirstOrDefault(c => c.Name == cmd);

		public Task InitializeAsync(ICitroid citroid)
		{
			Commands.Add(new CommandEcho());
			Commands.Add(new CommandCalc());
			return Task.Delay(0);
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
				if ((token != Token.Args) && (char.IsControl(c) || char.IsSeparator(c) || char.IsWhiteSpace(c)))
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
			CommandNode node;
			try
			{
				node = ParseCommand(mes.text, citroid.GetUser(mes.user));
			}
			catch (ParseCommandException ex)
			{
				return;
			}
			if (node == null)
				return;

			ICommand cmd = GetCommand(node.Cmd);

			if (cmd == null)
				return;

			try
			{
				await citroid.PostAsync(mes.channel, cmd.Process(node.Args));
			}
			catch (Exception ex)
			{
				await citroid.PostAsync(mes.channel, $"コマンド内部でエラー({ex.GetType().Name}): {ex.Message}");
			}

		}
	}
}
