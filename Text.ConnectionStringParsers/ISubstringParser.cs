namespace NoP77svk.Text.ConnectionStringParsers
{
    public interface ISubstringParser
    {
        public void Parse(string? value);

        public string? Build();
    }
}
