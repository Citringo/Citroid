using System;

namespace CitroidForSlack
{
	/// <summary>
	/// Slack からエラーが返されたときに発生する例外。
	[Serializable]
	public class SlackException : Exception
	{
		public SlackException() { }
		public SlackException(string message) : base(message) { }
		public SlackException(string message, Exception inner) : base(message, inner) { }
		protected SlackException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}

}