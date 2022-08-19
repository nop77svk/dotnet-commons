namespace NoP77svk.Text.ConnectionStringParsers.Tests
{
    using NoP77svk.Text.ConnectionStringParsers;

    public class MockSubstringParser : ISubstringParser
    {
        public MockSubstringParser(string? value = null)
        {
            Value = value;
        }

        internal string? Value { get; set; }

        string? ISubstringParser.Build() => Value;

        void ISubstringParser.Parse(string? value)
        {
            Value = value;
        }
    }
}
