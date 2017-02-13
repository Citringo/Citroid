using System.Collections.Generic;

namespace CitroidForSlack
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
		/// 単語を分割する文字を登録します。
		/// </summary>
		public List<string> Divider { get; set; } = new List<string>
		{
			"て", "に", "を", "は", "が", "で", "と", "の", "や", "へ", "も",
			"こ", "そ", "あ", "ど", "、", "。", "，", "．", "！", "？"

		};


	}


}