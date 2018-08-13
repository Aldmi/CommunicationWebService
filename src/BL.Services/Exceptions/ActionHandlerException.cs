using System;
using System.Runtime.Serialization;

namespace BL.Services.Exceptions
{
    public class ActionHandlerException : Exception
    {
        public ActionHandlerException() { }
        public ActionHandlerException(string message) : base(message) { }
        public ActionHandlerException(string message, Exception inner) : base(message, inner) { }
        protected ActionHandlerException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}