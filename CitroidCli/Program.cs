using CitroidForSlack;
using CitroidForSlack.Plugins.CitDealer;
using CitroidForSlack.Plugins.Groorine;
using CitroidForSlack.Plugins.NazoBrain;
using CitroidForSlack.Plugins.Utiline;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CitroidCli
{
	class Program
	{
		class Config
		{
			public string Token { get; set; }
		}

		static void Main(string[] args)
		{
			Task t = MainAsync(args);
			if (t.IsFaulted)
			{
				t.Wait();
			}

			while (true)
			{
				Thread.Sleep(1);
			}
		}

		static async Task MainAsync(string[] args)
		{
			var config = new Config();

			if (File.Exists("config.json"))
				config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
			while (string.IsNullOrEmpty(config?.Token))
				config.Token = Input("Citroid へようこそ。Slack トークンを入力してください");


			Citroid cr = await Citroid.CreateAsync(config.Token, new MessageBotNazoBrain(), new MessageBotCitDealer(), new MessageBotGroorine(), new MessageBotUtiline());
			
		}

		static string Input(string prompt)
		{
			Console.Write(prompt);
			Console.Write(">> ");
			return Console.ReadLine();
		}
	}
}
