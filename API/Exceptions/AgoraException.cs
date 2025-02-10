using System;
using System.Runtime.Serialization;

namespace API.Exceptions;

public class AgoraException: Exception
{
    public AgoraException()
    {
    }

    protected AgoraException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public AgoraException(string? message) : base(message)
    {
    }

    public AgoraException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}