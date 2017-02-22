using System;
using System.Linq;

namespace CitroidForSlack
{
	/// <summary>
	/// 拡張メソッドをホストします。
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Unix タイムスタンプの始点を表す <see cref="DateTime"/>。
		/// </summary>
		private static DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		/// <summary>
		/// <see cref="DateTime"/> オブジェクトをUNIXタイムスタンプに変換します。
		/// </summary>
		/// <param name="self"></param>
		/// <returns></returns>
		public static long ToEpoch(this DateTime self)
		{
			return (long)((self.ToUniversalTime() - UNIX_EPOCH).TotalSeconds);
		}

		/// <summary>
		/// UNIXタイムスタンプを <see cref="DateTime"/> オブジェクトに変換します。
		/// </summary>
		/// <param name="self"></param>
		/// <returns></returns>
		public static DateTime ToDateTime(this long self)
		{
			return UNIX_EPOCH.AddSeconds(self);
		}

		public static string[] SplitWithNewLine(this string str) => str.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

	}
}