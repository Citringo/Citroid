using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CitroidForSlack.Utils
{
	/// <summary>
	/// 特殊な表記のタイプを指定します。
	/// </summary>
	public enum SpecialTextType
	{
		/// <summary>
		/// 普通のテキスト。
		/// </summary>
		Normal,
		/// <summary>
		/// 絵文字のマークアップ。
		/// </summary>
		Emoji,
		/// <summary>
		/// URL 文字列。
		/// </summary>
		Url
	}
	/// <summary>
	/// 状態に依存しない静的で開発において便利な機能を提供します。このクラスは継承できません。
	/// </summary>
	public static class StaticUtility
	{
		public static SpecialTextType CheckSpecialText(string text)
		{
			if (Regex.IsMatch(text, @"^https?://[\w/:%#\$&\?\(\)~\.=\+\-]+$")) return SpecialTextType.Url;
			if (Regex.IsMatch(text, @"^:[\w_-]+:$")) return SpecialTextType.Emoji;
			return SpecialTextType.Normal;

		}
	}
}
