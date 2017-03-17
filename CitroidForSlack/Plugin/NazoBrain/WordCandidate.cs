using System;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace CitroidForSlack.Plugins.NazoBrain
{

	[Serializable]
	public class WordCandidate : WordCandidateChild
	{
		
		public WordCandidate(string word) : this(word, new List<WordCandidateChild>(), DateTime.UtcNow) { }

		private List<WordCandidateChild> _candidates;

		public List<WordCandidateChild> Candidates
		{
			get
			{
				for (var i = 0; i < _candidates.Count; i++)
					if ((DateTime.UtcNow - _candidates[i].RegisteredTime).TotalHours > MessageBotNazoBrain.WORD_LIFE_SPAN)
						_candidates.Remove(_candidates[i]);
				return _candidates;
			}
			set
			{
				_candidates = value;
			}
		}

		[JsonConstructor]
		public WordCandidate(string word, List<WordCandidateChild> childlen, DateTime time) : base(word, time)
		{
			Candidates = childlen;
		}

		public WordCandidateChild Add(string c) => Add(c, DateTime.UtcNow);

		public WordCandidateChild Add(string c, DateTime time)
		{
			if (Candidates.FirstOrDefault(ca => ca.MyText == c) is WordCandidateChild wc)
			{
				Candidates.Add(wc);
				wc.RegisteredTime = time;
				return wc;
			}
			Candidates.Add(wc = new WordCandidateChild(c, time));
			return wc;
		}

	}

	[Serializable]
	public class WordCandidateChild
	{
		public string MyText { get; set; }
		public DateTime RegisteredTime { get; set; }



		public WordCandidateChild(string word) : this(word, DateTime.UtcNow) { }

		[JsonConstructor]
		public WordCandidateChild(string word, DateTime time)
		{
			MyText = word;
			RegisteredTime = time;
		}
	}
}