using System;
using System.Runtime.Serialization;

namespace Soultech.BasicAuthentication.Exceptions
{
    [Serializable]
    public class BasicAuthenticationException : Exception
    {
        public BasicAuthenticationException()
        {
        }

        public BasicAuthenticationException(string message) : base(message)
        {
        }

        public BasicAuthenticationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public BasicAuthenticationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}