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

		public string Version => "2.0.0 pre-pre-alpha";

		public string Help => "できることはありません。";

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
					break;
				default:
					var a = await citroid.PostAsync(mes.channel, "じゃあいくよ。 じゃんけん");
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

}
