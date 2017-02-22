using System;
using System.Collections.Generic;
using System.Linq;

namespace CitroidForSlack.Extensions
{
	public static class LinqExtension
	{
		private static Random _rand = new Random();
		public static T Random<T>(this IEnumerable<T> ie) => ie.Count() > 0 ? ie.ElementAt(_rand.Next(ie.Count())) : default(T);

		// http://stackoverflow.com/questions/5807128/an-extension-method-on-ienumerable-needed-for-shuffling
		// Thanks to LukeH
		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source) => source.Shuffle(new Random());

		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			if (rng == null) throw new ArgumentNullException(nameof(rng));

			return source.ShuffleIterator(rng);
		}

		private static IEnumerable<T> ShuffleIterator<T>(
			this IEnumerable<T> source, Random rng)
		{
			var buffer = source.ToList();
			for (var i = 0; i < buffer.Count; i++)
			{
				var j = rng.Next(i, buffer.Count);
				yield return buffer[j];

				buffer[j] = buffer[i];
			}
		}
	}

}
