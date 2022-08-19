namespace NoP77svk.Commons
{
    using System;

    public class ValueConstraintViolatedException
        : Exception
    {
        public ValueConstraintViolatedException()
        {
        }

        public ValueConstraintViolatedException(string message)
            : base(message)
        {
        }

        public ValueConstraintViolatedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
