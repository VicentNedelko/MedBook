using System;
using System.Runtime.Serialization;

namespace EmailService
{
    [Serializable]
    public class WrongEmailException : Exception
    {
        public WrongEmailException()
        {
        }

        public WrongEmailException(string message)
            : base(message)
        {
        }

        public WrongEmailException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected WrongEmailException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}