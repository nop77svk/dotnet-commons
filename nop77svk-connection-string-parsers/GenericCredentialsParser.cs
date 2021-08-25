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

        public string? Name { get; set; }
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
            {
                Name = null;
                Password = null;
            }
            else
            {
                int namePasswordDelimiterIx = value.IndexOf(NamePasswordDelimiter);
                if (namePasswordDelimiterIx < 0)
                {
                    Name = value;
                    Password = null;
                }
                else if (namePasswordDelimiterIx == 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Parsing empty username with non-empty password? What are you trying to achieve?");
                }
                else
                {
                    Name = value.Substring(0, namePasswordDelimiterIx);
                    Password = value.Substring(namePasswordDelimiterIx + NamePasswordDelimiter.Length);
                }
            }
        }
    }
}
