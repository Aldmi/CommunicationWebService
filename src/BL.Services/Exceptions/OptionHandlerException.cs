using System;
using System.Runtime.Serialization;

namespace BL.Services.Exceptions
{
    public class OptionHandlerException : ActionHandlerException
    {
        public OptionHandlerException(){ }
        public OptionHandlerException(string message) : base(message) { }
        public OptionHandlerException(string message, Exception inner) : base(message, inner) { }
        protected OptionHandlerException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}