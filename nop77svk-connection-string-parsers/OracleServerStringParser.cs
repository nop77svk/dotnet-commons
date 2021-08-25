namespace NoP77svk.Data.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class OracleServerStringParser
        : ISubstringParser
    {
        public OracleServerStringParser(string hostPortDelimiter = ":", string serviceOrSidDelimiter = "/")
        {
            HostPortDelimiter = hostPortDelimiter;
            ServiceOrSidDelimiter = serviceOrSidDelimiter;
        }

        public string HostPortDelimiter { get; } = ":";
        public string ServiceOrSidDelimiter { get; } = "/";

        public string? Host { get; set; }
        public string? Port { get; set; }
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

        protected ValueTuple<string?, string?> SplitByPortDelimiter(string? value)
        {
            string? returnHost;
            string? returnPort;

            if (value is null)
            {
                returnHost = null;
                returnPort = null;
            }
            else
            {
                int hostPortDelimiterIx = value.IndexOf(HostPortDelimiter);
                if (hostPortDelimiterIx < 0)
                {
                    returnHost = value;
                    returnPort = null;
                }
                else if (hostPortDelimiterIx == 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Parsing empty server host with non-empty port? What are you trying to achieve?");
                }
                else
                {
                    returnHost = value.Substring(0, hostPortDelimiterIx);
                    returnPort = value.Substring(hostPortDelimiterIx + HostPortDelimiter.Length);
                }
            }

            return new ValueTuple<string?, string?>(returnHost, returnPort);
        }
    }
}
