namespace NoP77svk.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Text;

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
