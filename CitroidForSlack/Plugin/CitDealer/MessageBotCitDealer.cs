using CitroidForSlack.Api;
using CitroidForSlack.Extensions;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace CitroidForSlack.Plugins.CitDealer
{
	public class MessageBotCitDealer : IMessageBot
	{
		public string Name => "しとディーラー";

		public string Copyright => "(C)2017 Citrine";

		public string Version => "2.0.0 pre-alpha";

		public string Help => $@"スローライフゲーム
現実と同じように時間があり、春夏秋冬のある世界「スラックアイル」に移住し、のんびり生活をおくる。";

		enum GameState
		{
			Inactive,
		}
		
		
		public void Exit(ICitroid citroid)
		{
			//TODO: ゲームの状態をシリアライズさせる
		}
		
		int count = 0;

		public async Task InitializeAsync(ICitroid citroid)
		{
			
		}


		public async Task RunAsync(Message mes, ICitroid citroid)
		{
			if (mes.subtype != null)
				return;

			if (!mes.text.Contains("しとげーむ"))
				return;

			switch (count)
			{
				case 0:
					await citroid.PostAsync(mes.channel, "... ゲームはまだできてないんだ。");
					break;
				case 1:
					await citroid.PostAsync(mes.channel, "... まだできてないんだってば。");
					break;
				case 2:
					await citroid.PostAsync(mes.channel, "しょうがないなあ、そんなに言うならじゃんけんでもするかい？");
					//await citroid.PostAsync(mes.channel, "しょうがないなあ、じゃあちょっと実験したいゲームがあるからやってくれるかい。");
					break;
				default:
					PostedMessage a = await citroid.PostAsync(mes.channel, "じゃあいくよ。 じゃんけん");
					//PostedMessage a = await citroid.PostAsync(mes.channel, "釣りゲームだよ。 魚がかかったら :a: ボタンを押してつるんだ。どうだ、やるかい？やるなら :a:ボタンを押してくれ。");
					//await a.AddReactionAsync("a");
					//count = 0;
					await a.AddReactionAsync("fist");
					await a.AddReactionAsync("v");
					await a.AddReactionAsync("hand");
					string Judge(string myhand, string win, string lose) => myhand == win ? "僕の勝ち！" : myhand == lose ? "君の勝ち！" : "あいこ！" ;
					a.ReactionAdded += async (emoji, user) =>
					{
						var te = new[] { "fist", "v", "hand" }.Random();
						var output = "";
						if (emoji == "fist")
							output = Judge(te, "hand", "v");
						else if (emoji == "v")
							output = Judge(te, "fist", "hand");
						else if (emoji == "hand")
							output = Judge(te, "v", "fist");
						else
							output = "なんだその手は！";
						a = await a.UpdateAsync($"君:{emoji}: :{te}:僕 {output}");
					};

					break;
			}
			count++;
		}

		#region どうでもいいやつ
		private ICitroid _citroid;

		public readonly Random Rand = new Random();

		private Timer _timer = new Timer();

		public bool CanExecute(Message mes) => string.IsNullOrEmpty(mes.subtype);
		#endregion
	}

	public class SaveData
	{

	}

	public class Player : Person
	{
		public string SlackId { get; }
		public Player(string name, string slackId, Gender gender, int money = 0) : base(name, gender, Personality.Nerd, money)
		{
			SlackId = slackId;
		}
	}

	public abstract class Person : IPerson
	{
		public int Money { get; set; }

		public string Name { get; }

		public Gender Gender { get; }

		public Personality Personality { get; }

		public Person(string name, Gender gender, Personality pers, int money = 0)
		{
			Name = name;
			Gender = gender;
			Personality = pers;
			Money = money;
		}
	}



	public class World
	{
		private World() { }

		public async Task<World> CreateWorldAsync()
		{
			return new World();
		}
	}


}
