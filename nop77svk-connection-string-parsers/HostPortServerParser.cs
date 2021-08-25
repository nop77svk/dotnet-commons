namespace NoP77svk.Data.Utils
{
    using System;

    public class HostPortServerParser
        : ISubstringParser
    {
        public HostPortServerParser(string hostPortDelimiter = ":")
        {
            HostPortDelimiter = hostPortDelimiter;
        }

        public string HostPortDelimiter { get; } = ":";

        public string? Host { get; set; }
        public string? Port { get; set; }

        string? ISubstringParser.Build()
        {
            if (Host is null)
                return null;

            if (Port is null)
                return Host;
            else
                return Host + HostPortDelimiter + Port;
        }

        void ISubstringParser.Parse(string? value)
        {
            if (value is null)
                (Host, Port) = (null, null);
            else
                (Host, Port) = SplitByPortDelimiter(value);
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
