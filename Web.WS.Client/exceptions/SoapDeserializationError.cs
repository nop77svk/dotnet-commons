namespace NoP77svk.Web.WS
{
    using System;

    public class SoapDeserializationError
        : Exception
    {
        public SoapDeserializationError(string? message)
            : base(message)
        {
        }

        public SoapDeserializationError(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}
