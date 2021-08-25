namespace NoP77svk.Data.Utils
{
    using System;

    public class OracleCredentialsParser
        : GenericCredentialsParser
    {
        public const string OpeningProxyEncloser = "[";
        public const string ClosingProxyEncloser = "]";

        public OracleCredentialsParser()
            : base(namePasswordDelimiter: "/")
        {
        }

        public virtual string? SchemaUser { get; set; }
        public virtual string? ProxyUser { get; set; }

        public override string? Name
        {
            get => BuildUserNameWithProxy();
            set
            {
                ParseUserNameWithProxy(value);
            }
        }

        private void ParseUserNameWithProxy(string? value)
        {
            if (value is null)
            {
                SchemaUser = null;
                ProxyUser = null;
            }
            else
            {
                int leftBracketIx = value.LastIndexOf(OpeningProxyEncloser);
                if (leftBracketIx >= 0)
                {
                    if (!value.EndsWith(ClosingProxyEncloser))
                        throw new ArgumentOutOfRangeException(nameof(value), value, "Unmatched opening bracket of proxy user name");

                    ProxyUser = value.Substring(0, leftBracketIx);
                    SchemaUser = value[(leftBracketIx + 1)..^1];
                }
                else
                {
                    if (value.EndsWith(ClosingProxyEncloser))
                        throw new ArgumentOutOfRangeException(nameof(value), value, "Unmatched closing bracket of proxy user name");

                    SchemaUser = value;
                    ProxyUser = null;
                }
            }
        }

        private string? BuildUserNameWithProxy()
        {
            if (SchemaUser is null || ProxyUser is null)
                return SchemaUser;
            else
                return ProxyUser + "[" + SchemaUser + "]";
        }
    }
}
