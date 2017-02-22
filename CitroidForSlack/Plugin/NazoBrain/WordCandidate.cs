using System;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace CitroidForSlack.Plugins.NazoBrain
{

    [Serializable]
	public struct WordCandidate
	{
		public string MyText { get; set; }
		public DateTime RegisteredTime { get; set; }

		public WordCandidate(string word) : this(word, DateTime.Now) { }

		[JsonConstructor]
		public WordCandidate(string word, DateTime time)
		{
			MyText = word;
			RegisteredTime = time;
		}
	}


}