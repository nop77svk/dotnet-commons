namespace NoP77svk.Data
{
    using System;

    public class ConnectionStringParser
    {
        public ConnectionStringParser(string userServerDelimiter = "@", ConnectionStringParserPartPriority parsePriority = ConnectionStringParserPartPriority.Server)
        {
            UserServerDelimiter = userServerDelimiter;
            ParsePriority = parsePriority;
        }

        public string UserServerDelimiter { get; }
        public ConnectionStringParserPartPriority ParsePriority { get; }

        public string? ConnectionString
        {
            get => BuildConnectionString();
            set
            {
                ParseConnectionString(value);
            }
        }

        public virtual string? User { get; set; }
        public virtual string? Server { get; set; }

        private void ParseConnectionString(string? value)
        {
            if (value is null)
            {
                User = null;
                Server = null;
            }
            else
            {
                int userServerDelimiterIx = value.IndexOf(UserServerDelimiter);
                if (userServerDelimiterIx < 0)
                {
                    if (ParsePriority == ConnectionStringParserPartPriority.User)
                    {
                        User = value;
                        Server = null;
                    }
                    else if (ParsePriority == ConnectionStringParserPartPriority.Server)
                    {
                        User = null;
                        Server = value;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(nameof(ConnectionString), $"Unrecognized priority user vs server (actual: \"{ParsePriority}\") during connection string parsing");
                    }
                }
                else
                {
                    User = value.Substring(0, userServerDelimiterIx);
                    Server = value.Substring(userServerDelimiterIx + UserServerDelimiter.Length);
                }
            }
        }

        private string? BuildConnectionString()
        {
            if (User is null)
                return null;
            else if (Server is null)
                return User;
            else if (User == string.Empty && Server == string.Empty)
                return string.Empty;
            else
                return User + UserServerDelimiter + Server;
        }
    }
}
