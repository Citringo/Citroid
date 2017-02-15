using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WebSocket4Net;
using System;
using System.Text.RegularExpressions;
using System.Text;

namespace CitroidForSlack
{

	public delegate void ReactionEventHandler(ICitroid sender, Reaction e);


	/// <summary>
	/// 拡張可能な Slack クライアント機能を提供します。
	/// </summary>
	public class Citroid : ICitroid
	{
		/// <summary>
		/// Slack Web APIの基底URLです。
		/// </summary>
		public static readonly string SLACK_WEBAPI_BASE_URL = "https://slack.com/api/";
		
		/// <summary>
		/// このBotの名前です。
		/// </summary>
		public static readonly string NAME = "Citroid";

		/// <summary>
		/// このBotのバージョンです。
		/// </summary>
		public static readonly string VERSION = "1.2.0beta";

		/// <summary>
		/// ヘルプの表示につかう線です。
		/// </summary>
		public static readonly string LINE = "--------------------------";

		/// <summary>
		/// ヘルプのヘッダーです。
		/// </summary>
		public static string HelpHeader =>
			$"{NAME} v{VERSION}\n" +
			$"{LINE}\n" +
			$"{NAME.ToLower()} help      => このヘルプを表示します\n" +
			$"{NAME.ToLower()} changelog => 更新履歴を表示します\n";

		public static string HelpFooter =>
			"(C)2017 Citrine with GitHub contributors\n" +
			"GitHub: ttps://github.com/citringo/citroid";

		public static string ChangeLog =>
			"v1.2.0:\n" +
			"  - CitDealer (beta) が登場 ―― Citroid に新たなエンターテイメント。" +
			"  - PostMessage API が更新され、メッセージの編集もメソッドチェーンで可能に。" +
			"  - NazoBrain のかむひーや, ぼんぼやーじゅ機能を改善" +
			"v1.1.0:\n" +
			"  - ヘルプ 組込みコマンドの追加\n" +
			"  - 更新履歴 組込みコマンドの追加\n" +
			"  - 淫夢要素は *ありません*\n" +
			"v1.0.0:\n" +
			"  - 初回公開";

		/// <summary>
		/// ユーザー名とIDの辞書です。
		/// </summary>
		public readonly Dictionary<string, string> UserDictionary = new Dictionary<string, string>();

		/// <summary>
		/// メソッドおよびクエリを指定して、Slack API への URL を取得します。
		/// </summary>
		/// <param name="method"></param>
		/// <param name="query"></param>
		/// <returns></returns>
		public static string GetApiUrl(string method, NameValueCollection query)
			=> $"{SLACK_WEBAPI_BASE_URL}{method}?{query.ToQueryString()}";
		
		/// <summary>
		/// インスタンスがガベージコレクションされるときにBotの終了処理を行います。
		/// </summary>
		~Citroid()
		{
			Close();
		}

		/// <summary>
		/// WebSocket の切断および、Botの終了処理を行います。Citroidの終了時に必ず呼び出します。
		/// </summary>
		public void Close()
		{
			ws?.Close();
			foreach (IBot bot in _bots)
				bot.Exit(this);
		}

		/// <summary>
		/// この Citroid がAPIへの接続に使用するトークンを取得します。
		/// </summary>
		public string Token { get; protected set; }

		/// <summary>
		/// 内部 ID からユーザー名を取得します。
		/// </summary>
		public string GetUser(string id)
		{
			if (UserDictionary.ContainsKey(id))
				return UserDictionary[id];
			return "NONAME";
		}

		/// <summary>
		/// この Citroid の ID を取得します。
		/// </summary>
		public string Id { get; private set; }

		/// <summary>
		/// Slack Web API へアクセスします。
		/// </summary>
		/// <param name="method">Slack Web API メソッド。</param>
		/// <param name="query">トークン以外に追加するクエリ。</param>
		/// <returns></returns>
		public async Task<JObject> RequestAsync(string method, NameValueCollection query = null)
		{
			if (query == null)
				query = new NameValueCollection();
			query.Add("token", Token ?? throw new InternalException("Token isn't initialized."));
			return JObject.Parse(await new WebClient().DownloadStringTaskAsync(GetApiUrl(method, query)));
		}
		
		/// <summary>
		/// アクティブ状態を取得または設定します。<see cref="true"/>のときはBotが動作可能です。
		/// </summary>
		public bool IsActive { get; set; } = true;

		/// <summary>
		/// Slack にメッセージを送信します。
		/// </summary>
		public async Task<PostedMessage> PostAsync(string channel, string text, string userName = "", string iconUrl = "", string iconEmoji = "")
		{
			return (await RequestAsync("chat.postMessage", new NameValueCollection
			{
				{ "channel", channel},
				{ "text", text },
				{"username", userName },
				{"as_user", string.IsNullOrEmpty(userName) ? "true" : "false" },
				{"icon_url", iconUrl ?? "" },
				{ "icon_emoji", iconEmoji ?? ""}
			})).ToObject<PostedMessage>().Roid(this);
		}

		/// <summary>
		/// Citroid を作成します。
		/// </summary>
		/// <param name="token">Slack トークン。</param>
		/// <param name="bots">注入する Bot。</param>
		/// <returns></returns>
		public static async Task<Citroid> CreateAsync(string token, params IBot[] bots)
		{
			var cr = new Citroid(token);
			await cr.InternalInitAsync(bots);
			return cr;

		}

		/// <summary>
		/// 内部の初期化メソッドです。
		/// </summary>
		/// <param name="bots"></param>
		/// <returns></returns>
		private async Task InternalInitAsync(params IBot[] bots)
		{
			
			JObject j = await RequestAsync("rtm.start");

			if (!j["ok"].Value<bool>())
				throw new SlackException("Couldn't connect to slack.");

			ws = new WebSocket(j["url"].Value<string>());
			Id = j["self"]["id"].Value<string>();

			JObject members = await RequestAsync("users.list");
			foreach (var ian in members["members"].Values<JObject>().Select(m => new { id = m["id"].Value<string>(), name = m["name"].Value<string>() }))
			{
				UserDictionary[ian.id] = ian.name;
			}

			ws.Opened += (s, e) => Debug.WriteLine("Connected to slack.");
			ws.Error += (s, e) => Debug.WriteLine("Connection Error!!!!!!!!");
			ws.Closed += (s, e) => Debug.WriteLine("Disconnected from slack.");

			foreach (IBot bot in bots)
			{
				if (bot == null)
					continue;
				await RegisterMessageBotAsync(bot);
			}
			// Recieved something
			ws.MessageReceived += async (s, e) =>
			{
				Debug.WriteLine("Event Recieved");
				var json = JObject.Parse(e.Message);
				var type = json["type"].Value<string>();
				switch (type)
				{
					case "message":
						{
							Message m = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<Message>(e.Message));

							if (!IsActive || m.subtype != null && m.subtype != "bot_message" && m.subtype != "me_message")
								break;

							// Ignore posts from bot and me
							if (m.bot_id != null && m.user == Id)
								break;

							if (await RunEmbeddedCommandAsync(m))
								break;

							foreach (IMessageBot bot in _bots)
								if (bot.CanExecute(m))
									await bot.RunAsync(m, this);

						}
						break;
				}
			};

			ws.Open();

		}

		public static readonly Regex EmbeddedCommandHelp = new Regex(@"^\s*citroid\s+help\s*$", RegexOptions.IgnorePatternWhitespace);
		public static readonly Regex EmbeddedCommandChangelog = new Regex(@"^\s*citroid\s+changelog\s*$");


		/// <summary>
		/// 組み込みコマンドが含まれているか検証し、存在すれば実行します。
		/// </summary>
		/// <param name="m">対象の発言。</param>
		/// <returns>組み込みコマンドが実行されれば<see cref="true"/>、されなければ<see cref="false"/>を返します。</returns>
		private async Task<bool> RunEmbeddedCommandAsync(Message m)
		{
			// static メソッドで十分だったなァ！！！！！！(殺意)
			if (EmbeddedCommandHelp.IsMatch(m.text))
			{
				var sb = new StringBuilder();
				sb.AppendLine(HelpHeader);
				sb.AppendLine(LINE);
				foreach (IBot bot in _bots)
				{
					sb.AppendLine($"{bot.Name} v{bot.Version}");
					sb.AppendLine(bot.Help);
					sb.AppendLine(LINE);
				}
				sb.AppendLine(HelpFooter);
				await PostAsync(m.channel, sb.ToString());
				return true;
			}
			// static メソッｄ(ry
			if (EmbeddedCommandChangelog.IsMatch(m.text))
			{
				await PostAsync(m.channel, ChangeLog);
				return true;
			}

			return false;
		}

		private async Task RegisterMessageBotAsync(IBot bot)
		{
			if (_bots.Contains(bot) || bot == null)
				return;
			_bots.Add(bot);
			await bot.InitializeAsync(this);
		}


		private Citroid(string token)
		{
			Token = token;

		}

		private readonly List<IBot> _bots = new List<IBot>();
		private WebSocket ws;

	}

}