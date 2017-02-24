using System;
using System.Collections.Generic;
using Newtonsoft.Json;

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


		public void Add(string c) => Candidates.Add(new WordCandidate(c));

		public void Add(string c, DateTime time) => Candidates.Add(new WordCandidate(c, time));


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