using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace CitroidForSlack
{

	public enum GameState
	{
		Inactive,
		Matching,
		ThrowTheDice,
		Move,
	}

	public class MessageBotCitDealer : IMessageBot
	{
		public string Name => "しとディーラー";

		public string Copyright => "(C)2017 Citrine";

		public string Version => "1.0.0 beta";
		
		public string Help =>
$@"人生ゲームもどき
*最大{PlayersCount}人* までプレイ可能！
参加する人は""しとゲームやります""って言ってね。最初に宣言されてから1分間の間、参加者を募るよ。

1分間の間に{PlayersCount}人集まったらすぐにゲームが始まるよ。1分間の間、誰も来なかったらゲームは中止。

あとはディーラーの指示に従ってね :heart:";

		public GameState State { get; private set; } = GameState.Inactive;

		public static int PlayersCount => 4;

		public IGrid[] Map { get; private set; }

		#region コマンドregex
		readonly Regex join = new Regex(@"(しと|シト)ゲーム\s*やります");
		readonly Regex exit = new Regex(@"(しと|シト)ゲーム\s*やめます");
		readonly Regex throwTheDice = new Regex(@"振る|投げる|なげる|ふる");
		#endregion

		public List<Player> Players { get; } = new List<Player>();

		private int _nowPlayerIndex;
		public int NowPlayerIndex
		{
			get => _nowPlayerIndex;
			set
			{
				if (value < 0)
					return;
				_nowPlayerIndex = value % Players.Count;
			}
		}

		public Player NowPlayer => Players[NowPlayerIndex];

		public void Exit(ICitroid citroid)
		{
			//TODO: ゲームの状態をシリアライズさせる
		}

		private string dealerChannel;

		public string DealerChannel => dealerChannel;

		public async Task InitializeAsync(ICitroid citroid)
		{
			_citroid = citroid;
			//TODO: シリアライズしたデータがあったらそれを読み込んでゲームを再開する
			_timer = new Timer(60 * 1000);
			_timer.Elapsed += async (sender, e) =>
			{
				if (dealerChannel == null)
					Console.WriteLine("ディーラーチャンネルが存在しない！");
				else
				{
					await SayAsync("タイムリミット！");
					if (Players.Count > 1)
					{
						await StartMainGameAsync(citroid);
					}
					else
					{
						await SayAsync("残念ながら人が集まらなかったので、ゲームは中止です。");
						State = GameState.Inactive;
					}
				}
				_timer.Stop();
			};
		}

		public int Turn { get; set; }

		void BuildMap()
		{
			var temp = new List<IGrid>
			{
				new GridMoney("おこづかいをもらう。", 3000),
				new GridMoney("おこづかいをもらう。", 5000),
				new GridMoney("おこづかいをもらう。", 2700),
				new GridMoney("おこづかいをもらう。", 6000),
				new GridMoney("おこづかいをもらう。", 4500),
				new GridMoney("おこづかいをもらう。", 5000),
				new GridMoney("街でお金を拾う。", 1000),
				new GridMoney("街でお金を拾う。", 1000),
				new GridMoney("街でお金を拾う。", 1000),
				new GridMoney("街でお金を拾う。", 1000),
				new GridMoney("街でお金を拾う。", 1000),
				new GridMoney("街でお金を拾う。", 1000),
				new GridMoney("街でお金を拾う。", 1000),
				new GridMoney("街でお金を拾う。", 1000),
				new GridMoney("街でお金を拾う。", 1000),
				new GridMoney("街でお金を拾う。", 1000),
				new GridMoney("街でお金を拾う。", 1000),
				new GridMoney("街でお金を拾う。", 1000),
				new GridMoney("街でお金を拾う。", 1000),
				new GridMoney("高級魚を釣り上げる。", 8000),
				new GridMoney("高級魚を釣り上げる。", 8000),
				new GridMoney("高級魚を釣り上げる。", 8000),
				new GridMoney("高級魚を釣り上げる。", 8000),
				new GridMoney("釣具を買って魚を釣ろうとするが、連れずに壊れてしまう。", -3000),
				new GridMoney("魚を釣り上げる。", 2500),
				new GridMoney("交番に届けた財布の落とし主が見つかり、1割もらう。", 1800),
				new GridMoney("動画の広告収入が入る。", 2000),
				new GridMoney("タンスからお金が出てくる。", 10000),
				new GridMoney("埋蔵金を掘り当てる。", 100000),
				new GridMoney("友人に依頼されて絵を描く。", 5000),
				new GridMoney("自分の曲が売れる。", 5000),
				new GridMoney("SNSのスタンプで儲ける。", 5000),
				new GridMoney("お年玉を親にこっそり使われる。", -10000),
				new GridMoney("財布を落とす。", -8000),
				new GridMoney("ビデオに出演する。", 50000),
				new GridMoney("旅行先でお金を盗まれる。", -600),
				new GridMoney("新しいパソコンを買う。", -60000),
				new GridMoney("格安スマホに乗り換えてトクをする。", 8000),
				new GridMoney("新作の怪獣映画を見に行く。", -1000),
				new GridMoney("新作のアニメ映画を見に行く。", -1000),
				new GridMoney("新しいゲームを購入する。", -4000),
				new GridMoney("ゲームでガチャを引く。", -5000),
				new GridMoney("ゲームのカードを落とす。", -3000),
				new GridMoney("新種の生き物を見つける。", 30000),
				new GridMoney("街でお金を拾う。", 1000),
				new GridMoney("交番に届けた財布の落とし主が見つかり、1割もらう。", 1800),
				new GridMoney("動画の広告収入が入る。", 2000),
				new GridMoney("タンスからお金が出てくる。", 10000),
				new GridMoney("埋蔵金を掘り当てる。", 100000),
				new GridMoney("友人に依頼されて絵を描く。", 5000),
				new GridMoney("自分の曲が売れる。", 5000),
				new GridMoney("SNSのスタンプで儲ける。", 5000),
				new GridMoney("お年玉を親にこっそり使われる。", -10000),
				new GridMoney("財布を落とす。", -8000),
				new GridMoney("ビデオに出演する。", 50000),
				new GridMoney("旅行先でお金を盗まれる。", -600),
				new GridMoney("新しいパソコンを買う。", -60000),
				new GridMoney("格安スマホに乗り換えてトクをする。", 8000),
				new GridMoney("新作の怪獣映画を見に行く。", -1000),
				new GridMoney("新作のアニメ映画を見に行く。", -1000),
				new GridMoney("新しいゲームを購入する。", -4000),
				new GridMoney("ゲームでガチャを引く。", -5000),
				new GridMoney("ゲームのカードを落とす。", -3000),
				new GridMoney("新種の生き物を見つける。", 30000),
				new GridMoneyToOther("{0} が結婚した！祝儀として、", 50000),
				new GridMoneyToOther("{0} の誕生日！プレゼントとして、", 5000),
				new GridMoneyToOther("{0} を怪我させてしまった。賠償金として、", 20000),
				new GridMoneyToOther("{0} に絵を描いてもらう。", 5000),
				new GridMoneyToOther("{0} のために絵を描く。", -5000),
				new GridMoneyToOther("{0} から怪しいビデオを買わされる。", 19810),
				new GridMoneyToOther("今日は誕生日！親友の{0}がプレゼントをくれる。", -5000),
				new GridBecomePenniless("新しい事業を始めた！しかし、赤字続きで破綻する。"),
				new GridBecomePenniless("会社が倒産する。"),
				new GridMakesSomeonePenniless("{0} が新しい事業を始めた。しかし、赤字続きで破綻する。"),
				new GridMakesSomeonePenniless("{0} の会社が倒産する。"),
				new GridMoney("魚を釣ろうとするが、何も釣れない。", 0),
				new GridMoney("魚を釣ろうとするが、何も釣れない。", 0),
				new GridMoney("魚を釣ろうとするが、何も釣れない。", 0),
				new GridMoney("魚を釣ろうとするが、何も釣れない。", 0),
				new GridMoney("一日中ごろごろする。", 0),
				new GridGotoStart("タイムマシンを発明したが、バグって戻れなくなる。"),
				new GridGotoStart("家に財布を忘れる。"),
				new GridGotoStart("コンロの火を消したか心配になる。"),
				new GridGotoStart("鍵を閉め忘れたか心配になる。"),
				new GridGotoStart("家に携帯電話:iphone:を忘れる。"),
				


			};
			temp.Shuffle(Rand);
			temp.Insert(0, new GridStart());
			temp.Add(new GridGoal());
			Map = temp.ToArray();
		}
		private async Task StartMainGameAsync(ICitroid citroid)
		{
			BuildMap();
			await SayAsync("参加者が十分揃ったのでゲームスタート！");
			//await SayAsync("といいたいところですが、残念ながら未実装です:fu:よって、ゲームは中止です。");
			//State = GameState.Inactive;
			State = GameState.ThrowTheDice;
			await GameHandleAsync(citroid);
		}

		private Task SayAsync(string mes) => _citroid.PostAsync(DealerChannel, mes);

		private async Task GameHandleAsync(ICitroid citroid)
		{
			var userName = citroid.GetUser(NowPlayer.Id);

			switch (State)
			{
				case GameState.ThrowTheDice:
					if (Turn % 4 == 0 && Turn != 0)
					{
						// 給料日イベント
						await SayAsync("給料日です！皆さんに10000円が支給されます！");
						foreach (Player p in Players)
							p.Money += 10000;
					}
					await SayAsync(
						$"{userName} さんの番です！\n" +
						"\"振る\" と言って、サイコロを振ってください");
					break;
				case GameState.Move:
					if (NowPlayer == null)
					{
						await ShowErrorAndEndGameAsync($"BUG! {nameof(NowPlayer)} が null");
						return;
					}
					var dice = Rand.Next(1, 6);
					await SayAsync($"{dice} が出ました！ {dice} 歩進みます");
					for (var i = 1; i <= dice; i++)
					{
						// 進む
						NowPlayer.Position++;
						await Task.Delay(300);
						if (Map[NowPlayer.Position].AlwaysStop)
							break;
					}
					await Map[NowPlayer.Position].RunAsync(this, citroid, NowPlayer);
					await SayAsync(GetList());
					if (Players.Count(p => !p.Reached) == 0)
					{
						// ゴールイベント
						await SayAsync("全員がゴールしました！順位を発表します！");
						await Task.Delay(1000);

						await SayAsync(GetRanking());
						await Task.Delay(1000);

						await SayAsync($"よって優勝は {_citroid.GetUser(Players.OrderByDescending(p => p.Money).First().Id)}！おめでとナス:eggplant:！");
						await Task.Delay(1000);

						await SayAsync("優勝者には"
						+ new[] { "ゆうあしの髪の毛", "竹輪の穴", "めるアイコンの縁取り部分の醤油炒め", "日曜工作 Citroid組立キット(定価￥2000)", "otyaの椅子", "麿のMacbook", "シトリンのPC", "かにのぬいぐるみ", "しりとんの墓", "みんちゃんの自画像", "燻製の油絵" }.Random()
						+"をプレゼント！(大嘘)");
						await Task.Delay(1000);
						await SayAsync("また遊んでねー！");
						State = GameState.Inactive;
						break;
					}
					NowPlayerIndex++;
					if (NowPlayerIndex == 0)
						Turn++;
					
					State = GameState.ThrowTheDice;
					await GameHandleAsync(citroid);
					break;
			}
		}

		private async Task ShowErrorAndEndGameAsync(string message)
		{
			await SayAsync($"エラー: {message}\nゲームを終了します");
			State = GameState.Inactive;
			_timer.Stop();
		}

		public async Task RunAsync(Message mes, ICitroid citroid)
		{
			if (join.IsMatch(mes.text))
			{
				switch (State)
				{
					case GameState.Inactive:
						Players.Clear();
						Players.Add(new Player { Id = mes.user });
						await citroid.PostAsync(mes.channel,
							$"1分間参加者募集します！{PlayersCount}人集まったらすぐに始まります！\n" +
							"\"しとゲームやめます\"といえば参加をやめられます\n\n" + GetMatchingList());
						State = GameState.Matching;
						dealerChannel = mes.channel;
						_timer.Start();
						break;
					case GameState.Matching:
						if (Exists(mes.user))
						{
							await citroid.PostAsync(mes.channel, $"{citroid.GetUser(mes.user)} はもういるなー");
							break;
						}
						Players.Add(new Player { Id = mes.user });
						await citroid.PostAsync(mes.channel,
							"はいよー\n" + GetMatchingList());
						if (Players.Count >= PlayersCount)
							await StartMainGameAsync(citroid);
						break;
					case GameState.ThrowTheDice:
					case GameState.Move:
						await citroid.PostAsync(mes.channel, "ゲームはもう始まってるよ！");
						return;
					default:
						await ShowErrorAndEndGameAsync($"BUG! {nameof(GameState)}が異常: {State}");
						return;
				}
			}
			if (exit.IsMatch(mes.text))
			{
				switch (State)
				{
					case GameState.Inactive:
						await citroid.PostAsync(mes.channel, "まだ誰もいないよ！");
						break;
					case GameState.Matching:
						if (Exists(mes.user))
						{
							Remove(mes.user);
							await citroid.PostAsync(mes.channel, "(´･_･`)ばいばい\n" + GetMatchingList());
							if (Players.Count == 0)
								await citroid.PostAsync(mes.channel, "誰もいないじゃないか(呆れ) だれか入って～");
						}
						else
						{
							await citroid.PostAsync(mes.channel, $"{citroid.GetUser(mes.user)} は元からいないなー");
						}
						break;
					case GameState.ThrowTheDice:
						await citroid.PostAsync(mes.channel, "ゲームの途中だよ！");
						break;
				}
			}
			if (throwTheDice.IsMatch(mes.text) && State == GameState.ThrowTheDice && NowPlayer != null && NowPlayer.Id == mes.user)
			{
				State = GameState.Move;
				await GameHandleAsync(citroid);
			}
		}

		#region どうでもいいやつ
		private ICitroid _citroid;

		public readonly Random Rand = new Random();

		private Timer _timer = new Timer();

		public bool CanExecute(Message mes) => string.IsNullOrEmpty(mes.subtype);
		#endregion
		#region ヘルパーメソッド
		/// <summary>
		/// プレイヤーを削除します。元からいなければ<see cref="true"/>を返します。
		/// </summary>
		/// <param name="id">削除するプレイヤーのID。</param>
		/// <returns>元からいなければ<see cref="true"/>、そうでなければ<see cref="false"/>。</returns>
		private bool Remove(string id)
		{
			Player target = Players.FirstOrDefault(p => p.Id == id);
			if (target != null)
			{
				Players.Remove(target);
				return false;
			}
			return true;
		}

		private bool Exists(string id) => Players.Exists(p => p.Id == id);

		private string GetMatchingList()
		{
			var sb = new StringBuilder();
			for (var i = 0; i < PlayersCount; i++)
			{
				sb.AppendLine($"{i + 1}. {(Players.Count > i ? _citroid.GetUser(Players[i].Id) : "")}");
			}
			return sb.ToString();
		}

		private string GetList()
		{
			var sb = new StringBuilder();
			foreach (var p in Players.Select((p, i) => new { p, i }))
			{
				sb.AppendLine($"{p.i + 1}: {p.p.ToString(_citroid)}" + (!p.p.Reached ? $"ゴールまであと {Map.Length - 1 - p.p.Position} 歩":""));
			}
			return sb.ToString();
		}

		private string GetRanking()
		{
			var temp = Players.OrderByDescending(p => p.Money).Select((p, i) => new { p, i });
			var sb = new StringBuilder();
			foreach (var p in temp)
			{
				sb.AppendLine($"{p.i + 1}位 {p.p.ToString(_citroid)}");
			}
			
			return sb.ToString();
		}

		#endregion
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

		/// <summary>
		/// ゴールにたどり着いているかどうかを取得または設定します。。
		/// </summary>
		public bool Reached { get; set; }

		/// <summary>
		/// 与えられた所持金を文字列化して返します。
		/// </summary>
		/// <param name="m"></param>
		/// <returns></returns>
		public static string MoneyToString(int m) => m < 0 ? $"借金 {Math.Abs(m)}円" : $"{m}円";

		/// <summary>
		/// この <see cref="Player"/> の現在の情報をシリアライズして返します。
		/// </summary>
		/// <param name="citroid"></param>
		/// <returns></returns>
		public string ToString(ICitroid citroid) => $"{(Reached ? ":triangular_flag_on_post:" : "")}{citroid.GetUser(Id)}  :moneybag:{MoneyToString(Money)}";

	}

	/// <summary>
	/// すべてのマスの抽象クラスです。
	/// </summary>
	public interface IGrid
	{
		bool AlwaysStop { get; }
		/// <summary>
		/// マスを踏んだときのイベントを実行します。
		/// </summary>
		Task RunAsync(MessageBotCitDealer dealer, ICitroid citroid, Player player);
	}

	public abstract class GridReasonBase : IGrid
	{
		/// <summary>
		/// 所持金に影響を及ぼした理由を取得します。
		/// </summary>
		public string Reason { get; }

		public GridReasonBase(string reason) => Reason = reason;

		public abstract bool AlwaysStop { get; }

		public abstract Task RunAsync(MessageBotCitDealer dealer, ICitroid citroid, Player player);
	}

	public class GridGotoStart : GridReasonBase
	{
		public GridGotoStart(string reason) : base(reason) { }

		public override bool AlwaysStop => false;

		public override async Task RunAsync(MessageBotCitDealer dealer, ICitroid citroid, Player player)
		{
			player.Position = 0;
			await citroid.PostAsync(dealer.DealerChannel, $"{Reason} 振り出しに戻る。");
		}
	}

	/// <summary>
	/// お金を手に入れる、または手放すマスを表します。
	/// </summary>
	public class GridMoney : GridReasonBase
	{
		/// <summary>
		/// このマスが影響を与える金額を取得します。正の整数なら獲得したお金、負の整数なら失ったお金です。
		/// </summary>
		public int Money { get; }

		/// <summary>
		/// <see cref="GridMoney"/> の新しいインスタンスを初期化します。
		/// </summary>
		public GridMoney(string reason, int money) : base(reason) => Money = money;

		/// <summary>
		/// イベントを実行します。
		/// </summary>
		public override async Task RunAsync(MessageBotCitDealer dealer, ICitroid citroid, Player player)
		{
			player.Money += Money;
			await citroid.PostAsync(dealer.DealerChannel, $"{Reason} {MoneyText}");
		}

		protected virtual string MoneyText => 
			Money < 0	? $"{Math.Abs(Money)}円はらう。" :
			Money == 0	? "" :
						  $"{Money}円もらう。";

		public override bool AlwaysStop => false;
	}

	/// <summary>
	/// お金を他のプレイヤーに渡す、または他のプレイヤーからもらうマスを表します。
	/// </summary>
	public class GridMoneyToOther : GridMoney
	{
		/// <summary>
		/// <see cref="GridMoneyToOther"/> の新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="reason">理由。相手プレイヤーの名前が入るところに {0} と入れること。</param>
		/// <param name="money">金額。正の整数なら相手にお金を渡す。負の整数なら相手からお金をもらう。</param>
		public GridMoneyToOther(string reason, int money) : base(reason, money) { }

		public override async Task RunAsync(MessageBotCitDealer dealer, ICitroid citroid, Player player)
		{
			Player p = dealer.Players.Where(pl => pl != player).Random();
			var pName = citroid.GetUser(p.Id);
			p.Money += Money; player.Money -= Money;
			var action = Money < 0 ? $"{pName}から{Money}円もらう。" : $"{pName}に{Money}円わたす。";
			await citroid.PostAsync(dealer.DealerChannel, $"{string.Format(Reason, pName)} {action}");
		}
	}

	/// <summary>
	/// スタート地点のマスです。何もイベントがありません。
	/// </summary>
	public class GridStart : IGrid
	{
		public async Task RunAsync(MessageBotCitDealer dealer, ICitroid citroid, Player player)
		{
			// nothing to do \('ω')\
		}
		public bool AlwaysStop => false;
	}

	/// <summary>
	/// ゴールのマスを表します。
	/// </summary>
	public class GridGoal : IGrid
	{
		/// <summary>
		/// 順位による金額の一覧。
		/// </summary>
		static int[] bonusList = {
			30000,
			20000,
			15000,
			10000,
			 5000,
			 2500,
			 1250,
			  700
		};
		///
		public async Task RunAsync(MessageBotCitDealer dealer, ICitroid citroid, Player player)
		{
			player.Reached = true;
			var rank = dealer.Players.Count(p => p.Reached);
			var bonus = bonusList[Math.Min(rank, MessageBotCitDealer.PlayersCount) - 1];

			await citroid.PostAsync(dealer.DealerChannel, $"{citroid.GetUser(player.Id)} ゴール！ おめでとー！\n" +
				$"{rank}位でゴールしたので、賞金 {bonus}円をプレゼント！");
			player.Money += bonus;
		}
		public bool AlwaysStop => true;
	}

	public class GridBecomePenniless : GridReasonBase
	{
		public GridBecomePenniless(string reason) : base(reason) { }

		public override bool AlwaysStop => false;

		public override async Task RunAsync(MessageBotCitDealer dealer, ICitroid citroid, Player player)
		{
			// 残　金　０　円
			player.Money = 0;
			// 悲しいなぁ...
			await citroid.PostAsync(dealer.DealerChannel, $"{Reason} あなたはお金を全部失う(絶望)");
		}
	}

	public class GridMakesSomeonePenniless : GridBecomePenniless
	{
		/// <summary>
		/// <see cref="GridMakesSomeonePenniless"/> の新しいインスタンスを初期化します。
		/// </summary>
		/// <param name="reason">理由。お金を失う相手の名前が入るところに、{0} と入れること。</param>
		public GridMakesSomeonePenniless(string reason) : base(reason) { }

		public override async Task RunAsync(MessageBotCitDealer dealer, ICitroid citroid, Player player)
		{
			Player p = dealer.Players.Where(pl => pl != player).Random();
			var pName = citroid.GetUser(p.Id);
			// 残　金　０　円
			p.Money = 0;
			// 悲しいなぁ...
			await citroid.PostAsync(dealer.DealerChannel, $"{string.Format(Reason, pName)} {pName} はお金を全部失う(絶望)");
		}
	}

	public static class LinqExtension
	{
		private static Random _rand = new Random();
		public static T Random<T>(this IEnumerable<T> ie) => ie.Count() > 0 ? ie.ElementAt(_rand.Next(ie.Count())) : default(T);

		// http://stackoverflow.com/questions/5807128/an-extension-method-on-ienumerable-needed-for-shuffling
		// Thanks to LukeH
		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source) => source.Shuffle(new Random());

		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			if (rng == null) throw new ArgumentNullException(nameof(rng));

			return source.ShuffleIterator(rng);
		}

		private static IEnumerable<T> ShuffleIterator<T>(
			this IEnumerable<T> source, Random rng)
		{
			var buffer = source.ToList();
			for (var i = 0; i < buffer.Count; i++)
			{
				var j = rng.Next(i, buffer.Count);
				yield return buffer[j];

				buffer[j] = buffer[i];
			}
		}
	}

}
