using System.Collections.Generic;

namespace CitroidForSlack.Plugins.NazoBrain
{
    public class NazoBrainConfig
	{
		/// <summary>
		/// Botの返事をリプライ限定にするかどうか取得または設定します。
		/// </summary>
		public bool ReplyOnly { get; set; } = true;
		/// <summary>
		/// <see cref="ReplyOnly"/> が <see cref="false"/> の場合に投稿する確率を 0.0~1.0の範囲で取得または設定します。
		/// </summary>
		public double PostRate { get; set; } = 0.5;
		
		/// <summary>
		/// Twitter 用のコンシューマーキーです。
		/// </summary>
		public string TwitterCK { get; set; }

		/// <summary>
		/// Twitter 用のコンシューマーシークレットです。
		/// </summary>
		public string TwitterCS { get; set; }
		
		public string TwitterAccessToken { get; set; }

		public string TwitterAccessSecret { get; set; }

		/// <summary>
		/// 単語の取得にMecabを使うかどうか。
		/// </summary>
		public bool UseMecab { get; set; } = true;

	}


}