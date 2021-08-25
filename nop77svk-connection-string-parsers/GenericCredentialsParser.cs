namespace NoP77svk.Data.Utils
{
    using System;

    public class GenericCredentialsParser
        : ISubstringParser
    {
        public GenericCredentialsParser(string namePasswordDelimiter = "/")
        {
            NamePasswordDelimiter = namePasswordDelimiter;
        }

        public string NamePasswordDelimiter { get; } = "/";

        public virtual string? Name { get; set; }
        public virtual string? Password { get; set; }

        public virtual string? Build()
        {
            if (Name is null)
                return null;
            else if (Password is null)
                return Name;
            else
                return (Name ?? string.Empty) + NamePasswordDelimiter + Password;
        }

        public virtual void Parse(string? value)
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
