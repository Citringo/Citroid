using System.Collections.Specialized;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CitroidForSlack.Api
{
	public interface ICitroid
	{
		/// <summary>
		/// この Citroid の ID を取得します。
		/// </summary>
		string Id { get; }

		/// <summary>
		/// アクティブ状態を取得または設定します。<see cref="true"/>のときはBotが動作可能です。
		/// </summary>
		bool IsActive { get; set; }

		/// <summary>
		/// この Citroid がAPIへの接続に使用するトークンを取得します。
		/// </summary>
		string Token { get; }

		/// <summary>
		/// 内部 ID からユーザー名を取得します。
		/// </summary>
		string GetUser(string id);

		/// <summary>
		/// Slack にメッセージを送信します。
		/// </summary>
		Task<PostedMessage> PostAsync(string channel, string text, string userName = "", string iconUrl = "", string iconEmoji = "");
		
		/// <summary>
		/// Slack Web API へアクセスします。
		/// </summary>
		/// <param name="method">Slack Web API メソッド。</param>
		/// <param name="query">トークン以外に追加するクエリ。</param>
		Task<JObject> RequestAsync(string method, NameValueCollection query = null);

		/// <summary>
		/// Slack にファイルをアップロードします。
		/// </summary>
		/// <param name="path">アップロードするファイルのパス。</param>
		/// <param name="title">ファイルにつけるタイトル。</param>
		/// <param name="channels">共有するチャンネル。複数指定可。</param>
		Task UploadFileAsync(string path, string title = "", params string[] channels);

		/// <summary>
		/// リアクションが追加されたときに発生するイベント。
		/// </summary>
		event ReactionEventHandler ReactionAdded;
		
		/// <summary>
		/// リアクションが削除されたときに発生するイベント。
		/// </summary>
		event ReactionEventHandler ReactionRemoved;

		/// <summary>
		/// ファイルが共有されたときに発生するイベント。
		/// </summary>
		event FileEventHandler FileShared;
	}
}