namespace NoP77svk.Data.Utils.Tests
{
    using NoP77svk.Data.Utils;

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
