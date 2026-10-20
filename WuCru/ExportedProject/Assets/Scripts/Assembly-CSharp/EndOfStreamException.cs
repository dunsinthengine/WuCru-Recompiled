using System;
using System.Runtime.Serialization;

[Serializable]
public class EndOfStreamException : Exception
{
	public EndOfStreamException()
	{
	}

	public EndOfStreamException(string message)
		: base(message)
	{
	}

	public EndOfStreamException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

	protected EndOfStreamException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
