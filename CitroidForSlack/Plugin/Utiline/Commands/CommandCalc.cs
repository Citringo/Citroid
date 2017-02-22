using CitroidForSlack.Plugins.Utiline.Api;
using NCalc;

namespace CitroidForSlack.Utiline.Commands
{
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
}
