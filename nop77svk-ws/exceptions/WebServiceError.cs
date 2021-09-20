namespace NoP77svk.Web.WS
{
    using System;

    public class WebServiceError : Exception
    {
        public WebServiceError(string? message)
            : base(message)
        {
        }

        public WebServiceError(string? message, Exception? innerException)
            : base(message, innerException)
        {
        }
    }
}
