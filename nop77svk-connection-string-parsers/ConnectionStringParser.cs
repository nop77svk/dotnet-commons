namespace NoP77svk.Text.ConnectionStringParsers
{
    using System;

    public class ConnectionStringParser<TUserParser, TServerParser>
        where TUserParser : ISubstringParser
        where TServerParser : ISubstringParser
    {
        public ConnectionStringParser(
            TUserParser userParser,
            TServerParser serverParser,
            string userServerDelimiter = "@",
            ConnectionStringParserPartPriority parsePriority = ConnectionStringParserPartPriority.Server
        )
        {
            UserParser = userParser;
            ServerParser = serverParser;

            UserServerDelimiter = userServerDelimiter;
            ParsePriority = parsePriority;
        }

        public string UserServerDelimiter { get; }
        public ConnectionStringParserPartPriority ParsePriority { get; }

        public TUserParser UserParser { get; }
        public TServerParser ServerParser { get; }

        public string? ConnectionString
        {
            get => Build();
            set
            {
                Parse(value);
            }
        }

        public string? User
        {
            get => UserParser.Build();
            set
            {
                UserParser.Parse(value);
            }
        }

        public string? Server
        {
            get => ServerParser.Build();
            set
            {
                ServerParser.Parse(value);
            }
        }

        private void Parse(string? value)
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

        private string? Build()
        {
            string? leftSide = User;
            string? rightSide = Server;

            if (leftSide is null)
                return null;
            else if (rightSide is null)
                return leftSide;
            else if (leftSide == string.Empty && rightSide == string.Empty)
                return string.Empty;
            else
                return leftSide + UserServerDelimiter + rightSide;
        }
    }
}
