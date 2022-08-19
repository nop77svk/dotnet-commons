namespace NoP77svk.Text.ConnectionStringParsers
{
    using System.Text;

    public class OracleServerStringParser
        : HostPortServerStringParser, ISubstringParser
    {
        public OracleServerStringParser(string hostPortDelimiter = ":", string serviceOrSidDelimiter = "/")
            : base(hostPortDelimiter)
        {
            ServiceOrSidDelimiter = serviceOrSidDelimiter;
        }

        public string ServiceOrSidDelimiter { get; } = "/";

        public string? ServiceOrSid { get; set; }
        public string? TnsSpecOrAlias { get; set; }

        string? ISubstringParser.Build()
        {
            if (Host is null)
                return null;

            if (TnsSpecOrAlias != null)
            {
                return TnsSpecOrAlias;
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(Host);

                if (Port != null)
                {
                    builder.Append(HostPortDelimiter);
                    builder.Append(Port);
                }

                if (ServiceOrSid != null)
                {
                    builder.Append(ServiceOrSidDelimiter);
                    builder.Append(ServiceOrSid);
                }

                return builder.ToString();
            }
        }

        void ISubstringParser.Parse(string? value)
        {
            if (value is null)
            {
                Host = null;
                Port = null;
                ServiceOrSid = null;
            }
            else
            {
                int hostPathDelimiterIx = value.IndexOf(ServiceOrSidDelimiter);
                if (hostPathDelimiterIx < 0)
                {
                    ServiceOrSid = null;
                    (Host, Port) = SplitByPortDelimiter(value);
                    if (Port is null)
                    {
                        if (Host is null)
                        {
                            TnsSpecOrAlias = null;
                        }
                        else if (Host.StartsWith('(') && Host.EndsWith(')'))
                        {
                            TnsSpecOrAlias = Host;
                            Host = null;
                        }
                    }
                    else
                    {
                        TnsSpecOrAlias = null;
                    }
                }
                else
                {
                    ServiceOrSid = value.Substring(hostPathDelimiterIx + ServiceOrSidDelimiter.Length);
                    (Host, Port) = SplitByPortDelimiter(value.Substring(0, hostPathDelimiterIx));
                }
            }
        }
    }
}
