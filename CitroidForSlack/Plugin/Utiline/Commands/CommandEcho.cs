using CitroidForSlack.Plugins.Utiline.Api;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CitroidForSlack.Utiline.Commands
{
	public class CommandEcho : ICommand
	{
		public string Name => "echo";

		public string Usage => $@"指定した文字列を発言します。
{Name}: <message>";

		public string Process(string[] args) => string.Join(",", args);
	}
}
