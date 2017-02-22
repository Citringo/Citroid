using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace CitroidForSlack.Extensions
{
	/// <summary>
	/// <see cref="NameValueCollection"/> にクエリ文字列への変換機能を追加します。
	/// </summary>
	public static class NameValueCollectionExtension
	{
		/// <summary>
		/// この <see cref="NameValueCollection"/> をクエリ文字列に変換します。
		/// </summary>
		/// <param name="nvc"></param>
		/// <returns></returns>
		public static string ToQueryString(this NameValueCollection nvc)
			=> string.Join("&", nvc.AllKeys.Select(x => x + "=" + HttpUtility.UrlEncode(nvc[x])));
	}
}