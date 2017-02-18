using NCalc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitroidForSlack
{
	public class CommandEcho : ICommand
	{
		public string Name => "echo";

		public string Usage => $"{Name}: <message>";

		public string Process(string[] args) => string.Join("", args);
	}

	public class CommandCalc : ICommand
	{
		public string Name => "calc";

		public string Usage => $"{Name}: <message>";

		public string Process(string[] args)
		{
			var expr = string.Join("", args);
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
}
