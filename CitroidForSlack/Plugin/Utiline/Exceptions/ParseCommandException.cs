using System;

namespace CitroidForSlack.Plugins.Utiline.Exceptions
{
	[Serializable]
	public class ParseCommandException : Exception
	{
		public ParseCommandException() { }
		public ParseCommandException(string message) : base(message) { }
		public ParseCommandException(string message, Exception inner) : base(message, inner) { }
		protected ParseCommandException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}