using System;

namespace CitroidForSlack.Plugins.Utiline.Exceptions
{
		[Serializable]
		public class IllegalCommandCallException : Exception
		{
			public IllegalCommandCallException() { }
			public IllegalCommandCallException(string message) : base(message) { }
			public IllegalCommandCallException(string message, Exception inner) : base(message, inner) { }
			protected IllegalCommandCallException(
			  System.Runtime.Serialization.SerializationInfo info,
			  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
		}
}
