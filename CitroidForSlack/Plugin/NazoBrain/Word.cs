using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CitroidForSlack
{
    [Serializable]
	public class Word
	{
		public string MyText { get; set; }

		private List<WordCandidate> _candidates;

		public List<WordCandidate> Candidates
		{
			get
			{
				for (var i = 0; i < _candidates.Count; i++)
					if ((DateTime.UtcNow - _candidates[i].RegisteredTime).TotalHours > 24)
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


		public Word(string c) : this(c, new List<WordCandidate>()) { }

		[JsonConstructor]
		public Word(string myChar, List<WordCandidate> candidates)
		{
			MyText = myChar;
			_candidates = candidates ?? throw new ArgumentNullException(nameof(candidates));
		}
	}


}