using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitroidForSlack.Plugins.NazoBrain
{
	using static UnicodeBlock;
	static class NazoBrainExtension
	{
		/// <summary>
		/// この文字が属している Unicode ブロックを返します。
		/// </summary>
		/// <param name="c"></param>
		/// <returns></returns>
		public static UnicodeBlock GetBlock(this char c)
		{
			if (c <= '\u02af')
				return Laten;
			if ('\u1d00' <= c && c <= '\u2bff')
				return Symbol;
			if ('\u3040' <= c && c <= '\u309f')
				return Hiragana;
			if ('\u30a0' <= c && c <= '\u30ff')
				return Katakana;
			if ('\u3400' <= c && c <= '\u4dbf')
				return Kanji;
			if ('\u4e00' <= c && c <= '\u9fff')
				return Kanji;
			if ('\u3400' <= c && c <= '\u4dbf')
				return Kanji;
			if ('\uf900' <= c && c <= '\ufaf0')
				return Kanji;
			if ('\uff00' <= c && c <= '\uff60')
				return ZenkakuLatin;
			if ('\uff61' <= c && c <= '\uff9f')
				return HankakuKatakana;
			return Other;
		}
	}
}