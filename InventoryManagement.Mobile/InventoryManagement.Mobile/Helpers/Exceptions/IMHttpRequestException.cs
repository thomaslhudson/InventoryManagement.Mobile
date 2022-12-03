using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace InventoryManagement.Mobile
{
    [Serializable]
    public class IMHttpRequestException : Exception
    {
        private readonly System.Net.HttpStatusCode _httpStatusCode;

        public IMHttpRequestException()
        {
        }

        public IMHttpRequestException(string message)
            : base(message)
        {
        }

        public IMHttpRequestException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public IMHttpRequestException(string message, System.Net.HttpStatusCode httpStatusCode)
            : base(message)
        {
            _httpStatusCode = httpStatusCode;
        }

        public IMHttpRequestException(string message, Exception inner, System.Net.HttpStatusCode httpStatusCode)
            : base(message, inner)
        {
            _httpStatusCode = httpStatusCode;
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        // Constructor should be protected for unsealed classes, private for sealed classes.
        // (The Serializer invokes this constructor through reflection, so it can be private)
        protected IMHttpRequestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _httpStatusCode = (System.Net.HttpStatusCode)info.GetValue("HttpStatusCode", typeof(System.Net.HttpStatusCode));
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            info.AddValue("httpStatusCode", _httpStatusCode);

            // Must call the base class to let it save it's state
            base.GetObjectData(info, context);
        }

        public System.Net.HttpStatusCode HttpStatusCode
        {
            get
            {
                return _httpStatusCode;
            }
        }
    }
}
