namespace NoP77svk.Data.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class GenericCredentialsParser
        : ISubstringParser
    {
        public GenericCredentialsParser(string namePasswordDelimiter = "/")
        {
            NamePasswordDelimiter = namePasswordDelimiter;
        }

        public string NamePasswordDelimiter { get; } = "/";

        public virtual string? Name { get; set; }
        public string? Password { get; set; }

        string? ISubstringParser.Build()
        {
            if (Name is null)
                return null;
            else if (Password is null)
                return Name;
            else
                return (Name ?? string.Empty) + NamePasswordDelimiter + Password;
        }

        void ISubstringParser.Parse(string? value)
        {
            if (value is null)
                (Name, Password) = (null, null);
            else
                (Name, Password) = SplitByPasswordDelimiter(value);
        }

        protected ValueTuple<string?, string?> SplitByPasswordDelimiter(string? value)
        {
            string? returnName;
            string? returnPassword;

            if (value is null)
            {
                returnName = null;
                returnPassword = null;
            }
            else
            {
                int namePasswordDelimiterIx = value.IndexOf(NamePasswordDelimiter);
                if (namePasswordDelimiterIx < 0)
                {
                    returnName = value;
                    returnPassword = null;
                }
                else if (namePasswordDelimiterIx == 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Parsing empty username with non-empty password? What are you trying to achieve?");
                }
                else
                {
                    returnName = value.Substring(0, namePasswordDelimiterIx);
                    returnPassword = value.Substring(namePasswordDelimiterIx + NamePasswordDelimiter.Length);
                }
            }

            return new ValueTuple<string?, string?>(returnName, returnPassword);
        }
    }
}
