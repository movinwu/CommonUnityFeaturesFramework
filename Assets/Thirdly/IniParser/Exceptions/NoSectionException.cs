using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace INIParser.Exceptions
{
    class NoSectionException : Exception
    {
        public NoSectionException()
        {
        }

        public NoSectionException(string message) : base(message)
        {
        }

        public NoSectionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoSectionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
