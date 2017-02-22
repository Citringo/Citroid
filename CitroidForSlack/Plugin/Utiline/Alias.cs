namespace CitroidForSlack.Plugins.Utiline
{
	public class Alias
	{
		public string Name { get; set; }
		public string CommandName { get; set; }
		public string[] Arguments { get; set; }

		public Alias(string name, string cmdName, string[] args)
		{
			Name = name;
			CommandName = cmdName;
			Arguments = args;
		}
	}
}
