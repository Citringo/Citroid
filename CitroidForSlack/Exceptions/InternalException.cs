using System;

namespace CitroidForSlack.Exceptions
{

	/// <summary>
	/// 内部で発生する例外。
	/// </summary>
	[Serializable]
	public class InternalException : Exception
	{
		public InternalException() { }
		public InternalException(string message) : base(message) { }
		public InternalException(string message, Exception inner) : base(message, inner) { }
		protected InternalException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}

}