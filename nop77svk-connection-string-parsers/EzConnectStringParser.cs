namespace NoP77svk.Data.Utils
{
    using System;
    using System.Text;

    public class EzConnectStringParser
        : ConnectionStringParser
    {
        public EzConnectStringParser(string userServerDelimiter = "@", ConnectionStringParserPartPriority parsePriority = ConnectionStringParserPartPriority.Server, string namePasswordDelimiter = "/", string hostPortDelimiter = ":", string hostPathDelimiter = "/")
            : base(userServerDelimiter, parsePriority)
        {
            NamePasswordDelimiter = namePasswordDelimiter;
            HostPortDelimiter = hostPortDelimiter;
            HostPathDelimiter = hostPathDelimiter;
        }

        public string NamePasswordDelimiter { get; } = "/";
        public string HostPortDelimiter { get; } = ":";
        public string HostPathDelimiter { get; } = "/";

        public override string? User
        {
            get => BuildUser();
            set
            {
                ParseUser(value);
            }
        }

        public string? UserName { get; set; }
        public string? UserPassword { get; set; }

        public override string? Server
        {
            get => BuildServer();
            set
            {
                ParseServer(value);
            }
        }

        public string? ServerHost { get; set; }
        public string? ServerPort { get; set; }
        public string? ServerPath { get; set; }

        private void ParseServer(string? value)
        {
            if (value is null)
            {
                ServerHost = null;
                ServerPort = null;
                ServerPath = null;
            }
            else
            {
                int hostPathDelimiterIx = value.IndexOf(HostPathDelimiter);
                if (hostPathDelimiterIx < 0)
                {
                    ServerPath = null;
                    ParseServerWithoutPath(value);
                }
                else
                {
                    ServerPath = value.Substring(hostPathDelimiterIx + HostPathDelimiter.Length);
                    ParseServerWithoutPath(value.Substring(0, hostPathDelimiterIx));
                }
            }
        }

        private void ParseServerWithoutPath(string? value)
        {
            if (value is null)
            {
                ServerHost = null;
                ServerPort = null;
            }
            else
            {
                int hostPortDelimiterIx = value.IndexOf(HostPortDelimiter);
                if (hostPortDelimiterIx < 0)
                {
                    ServerHost = value;
                    ServerPort = null;
                }
                else if (hostPortDelimiterIx == 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(Server), "Parsing empty server host with non-empty port? What are you trying to achieve?");
                }
                else
                {
                    ServerHost = value.Substring(0, hostPortDelimiterIx);
                    ServerPort = value.Substring(hostPortDelimiterIx + HostPortDelimiter.Length);
                }
            }
        }

        private string? BuildServer()
        {
            if (ServerHost is null)
                return null;

            StringBuilder builder = new StringBuilder();
            builder.Append(ServerHost);

            if (ServerPort != null)
            {
                builder.Append(HostPortDelimiter);
                builder.Append(ServerPort);
            }

            if (ServerPath != null)
            {
                builder.Append(HostPathDelimiter);
                builder.Append(ServerPath);
            }

            return builder.ToString();
        }

        private void ParseUser(string? value)
        {
            if (value is null)
            {
                UserName = null;
                UserPassword = null;
            }
            else
            {
                int namePasswordDelimiterIx = value.IndexOf(NamePasswordDelimiter);
                if (namePasswordDelimiterIx < 0)
                {
                    UserName = value;
                    UserPassword = null;
                }
                else if (namePasswordDelimiterIx == 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(User), "Parsing empty username with non-empty password? What are you trying to achieve?");
                }
                else
                {
                    UserName = value.Substring(0, namePasswordDelimiterIx);
                    UserPassword = value.Substring(namePasswordDelimiterIx + NamePasswordDelimiter.Length);
                }
            }
        }

        private string? BuildUser()
        {
            if (UserName is null)
                return null;
            else if (UserPassword is null)
                return UserName;
            else
                return (UserName ?? string.Empty) + NamePasswordDelimiter + UserPassword;
        }
    }
}
