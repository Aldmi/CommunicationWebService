using System;
using System.Runtime.Serialization;

namespace BL.Services.Exceptions
{
    public class StorageHandlerException : Exception
    {
        public StorageHandlerException(){ }
        public StorageHandlerException(string message) : base(message) { }
        public StorageHandlerException(string message, Exception inner) : base(message, inner) { }
        protected StorageHandlerException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}