using System.Collections.Specialized;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CitroidForSlack
{
	public interface ICitroid
	{
		string Id { get; }
		bool IsActive { get; set; }
		string Token { get; }

		void Close();
		string GetUser(string id);
		Task PostAsync(string channel, string text, string userName = "", string iconUrl = "", string iconEmoji = "");
		Task<JObject> RequestAsync(string method, NameValueCollection query = null);
	}
}