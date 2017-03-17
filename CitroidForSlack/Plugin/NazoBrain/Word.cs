using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace CitroidForSlack.Plugins.NazoBrain
{
    [Serializable]
	public class Word
	{
		public string MyText { get; set; }

		public DateTime TimeStamp { get; set; }

		private List<WordCandidate> _candidates;

		public List<WordCandidate> Candidates
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


		public WordCandidate Add(string c) => Add(c, DateTime.UtcNow);

		public WordCandidate Add(string c, DateTime time)
		{
			if (Candidates.FirstOrDefault(ca => ca.MyText == c) is WordCandidate wc)
			{
				Candidates.Add(wc);
				wc.RegisteredTime = time;
				return wc;
			}
			Candidates.Add(wc = new WordCandidate(c, new List<WordCandidateChild>(), time));
			return wc;
		}


		public Word(string c) : this(c, new List<WordCandidate>(), DateTime.UtcNow) { }

		[JsonConstructor]
		public Word(string myChar, List<WordCandidate> candidates, DateTime datetime = default(DateTime))
		{
			MyText = myChar;
			_candidates = candidates ?? throw new ArgumentNullException(nameof(candidates));
			TimeStamp = datetime.Equals(default(DateTime)) ? DateTime.UtcNow : datetime;
		}
	}


}