using CitroidForSlack;
using CitroidForSlack.Plugins.CitDealer;
using CitroidForSlack.Plugins.Groorine;
using CitroidForSlack.Plugins.NazoBrain;
using CitroidForSlack.Plugins.Utiline;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace CitroidGui
{
	public partial class Form1 : Form
	{

		class Config
		{
			public string Token { get; set; }
		}

		public Form1()
		{
			InitializeComponent();
			MessageBotNazoBrain bot = null;
			Enabled = false;
			Citroid cr = null;
			Visible = false;
			var config = new Config();
			void update()
			{
				if (bot == null)
					return;
				treeView1.BeginUpdate();
				treeView1.Nodes.Clear();
				foreach (KeyValuePair<string, 
					Word> k in bot.WordBrain)
				{
					var t = new TreeNode($"Word {k.Value.MyText}");
					foreach (WordCandidate wc in k.Value.Candidates)
						t.Nodes.Add($"WordCandidate {wc.MyText}, {wc.RegisteredTime}");
					treeView1.Nodes.Add(t);
				}
				treeView1.EndUpdate();
			}

			Load += async (sender, e) =>
			{

				if (File.Exists("config.json"))
					config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
				while (string.IsNullOrEmpty(config?.Token))
					config.Token = Interaction.InputBox("Citroid へようこそ。\r\n今すぐトークンを入力して、Slack エクスペリエンスを体感しよう。", "Citroid");
				File.WriteAllText("config.json", JsonConvert.SerializeObject(config));
				cr = await Citroid.CreateAsync(config.Token, bot = new MessageBotNazoBrain(), new MessageBotCitDealer(), new MessageBotGroorine(), new MessageBotUtiline());
				Enabled = true;
				update();
				trackBar1.Enabled = !(checkBox1.Checked = bot.Config.ReplyOnly);
				trackBar1.Value = (int)(bot.Config.PostRate * 100);
				
				Show();
			};

			button1.Click += (sender, e) => bot?.Clear();

			button2.Click += (sender, e) =>
			{
				bot?.Learn(textBox1.Text, DateTime.Now);
				textBox1.Clear();
			};

			button3.Click += (sender, e) => update();

			FormClosing += (sender, e) =>
			{
				if (cr != null)
					cr.Close();
				
			};

			trackBar1.ValueChanged += (sender, e) =>
			{
				if (bot != null)
					bot.Config.PostRate = trackBar1.Value * 0.01;
			};

			checkBox1.CheckedChanged += (sender, e) =>
			{
				if (bot != null)
					trackBar1.Enabled = !(bot.Config.ReplyOnly = checkBox1.Checked);
			};

			button4.Click += async (sender, e) =>
			{
				if (bot != null)
					await bot.LearnFromSlack(cr);
			};

			AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
			{
				var ex = ((Exception)e.ExceptionObject);
				MessageBox.Show($@"バグかなんかでエラーが起きました！  Ctrl + C でコピーし、作者におしえてください！！
{ex.GetType().Name}: {ex.Message}
Stack Trace: {ex.StackTrace}
{Environment.OSVersion} {(Environment.Is64BitOperatingSystem ? "64Bit" : "")}
.NET Version: {Environment.Version}
Machine Name: {Environment.MachineName} UserName: {Environment.UserName}
CPU Count: {Environment.ProcessorCount} Tick: {Environment.TickCount}");
				
			};


		}
	}
}
