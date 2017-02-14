using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace CitroidForSlack
{
	public class MessageBotCitDealer : IMessageBot
	{
		public string Help => "人生ゲームもどき\n" +
			"*最大8人* までプレイ可能！\n" +
			"参加する人は\"しとゲームやります\"って言ってね。最初に宣言されてから1分間の間、参加者を募るよ。\n\n" +
			"1分間の間に8人集まったらすぐにゲームが始まるよ。1分間の間、誰も来なかったらゲームは中止。\n\n" +
			"そっから先は未実装:fu:";

		public enum GameState
		{
			Inactive,
			Matching,
			ThrowTheDice,
			
		}

		public string Name => "しとディーラー";

		public string Copyright => "(C)2017 Citrine";

		public string Version => "1.0.0 pre-alpha";

		public bool CanExecute(Message mes) => string.IsNullOrEmpty(mes.subtype);

		public GameState State { get; private set; } = GameState.Inactive;

		readonly Regex join = new Regex(@"(しと|シト)ゲーム\s*やります");
		readonly Regex exit = new Regex(@"(しと|シト)ゲーム\s*やめます");

		private readonly List<Player> _players = new List<Player>(8);

		private Timer timer = new Timer();

		public void Exit(ICitroid citroid)
		{
			//TODO: ゲームの状態をシリアライズさせる
		}

		private string dealerChannel;

		public async Task InitializeAsync(ICitroid citroid)
		{
			//TODO: シリアライズしたデータがあったらそれを読み込んでゲームを再開する
			timer = new Timer(60 * 1000);
			timer.Elapsed += async (sender, e) =>
			{
				if (dealerChannel == null)
					Console.WriteLine("ディーラーチャンネルが存在しない！");
				else
				{
					await citroid.PostAsync(dealerChannel, "タイムリミット！");
					if (_players.Count > 1)
					{
						await citroid.PostAsync(dealerChannel, "参加者が十分揃ったのでゲームスタート！");
						await citroid.PostAsync(dealerChannel, "といいたいところですが、残念ながら未実装です:fu:よって、ゲームは中止です。");
						State = GameState.Inactive;
					}
					else
					{
						await citroid.PostAsync(dealerChannel, "残念ながら人が集まらなかったので、ゲームは中止です。");
						State = GameState.Inactive;
					}
				}
				timer.Stop();
			};
		}

		/// <summary>
		/// プレイヤーを削除します。元からいなければ<see cref="true"/>を返します。
		/// </summary>
		/// <param name="id">削除するプレイヤーのID。</param>
		/// <returns>元からいなければ<see cref="true"/>、そうでなければ<see cref="false"/>。</returns>
		private bool Remove(string id)
		{
			Player target = _players.FirstOrDefault(p => p.Id == id);
			if (target != null)
			{
				_players.Remove(target);
				return false;
			}
			return true;
		}

		private string GetList(ICitroid c)
		{
			var sb = new StringBuilder();
			for (var i = 0; i < 8; i++)
			{
				sb.AppendLine($"{i + 1}. {(_players.Count > i ? c.GetUser(_players[i].Id) : "")}");
			}
			return sb.ToString();
		}

		public async Task RunAsync(Message mes, ICitroid citroid)
		{
			if (join.IsMatch(mes.text))
			{
				switch (State)
				{
					case GameState.Inactive:
						_players.Add(new Player { Id = mes.user });
						await citroid.PostAsync(mes.channel, 
							"1分間参加者募集します！8人集まったらすぐに始まります！\n" +
							"\"しとゲームやめます\"といえば参加をやめられます\n\n" + GetList(citroid));
						State = GameState.Matching;
						dealerChannel = mes.channel;
						timer.Start();
						break;
					case GameState.Matching:
						_players.Add(new Player { Id = mes.user });
						await citroid.PostAsync(mes.channel,
							"はいよー\n" + GetList(citroid));
						
						break;
					case GameState.ThrowTheDice:
						await citroid.PostAsync(mes.channel, "ゲームはもう始まってるよ！");
						return;
					default:
						await citroid.PostAsync(mes.channel, "バグ！ゲーム状態がおかしい");
						return;
				}
			}
			if (exit.IsMatch(mes.text))
			{
				await citroid.PostAsync(mes.channel, "未実装！！");
			}
		}
	}

	public class Player
	{
		/// <summary>
		/// プレイヤーを操作するユーザーのIDを取得します。
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// プレイヤーの所持金を取得または設定します。
		/// </summary>
		public int Money { get; set; }

		/// <summary>
		/// プレイヤーが今いるコマの位置を取得または設定します。
		/// </summary>
		public int Position { get; set; }

	}

}
