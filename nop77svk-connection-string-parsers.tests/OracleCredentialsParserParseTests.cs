namespace NoP77svk.Data.Utils.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NoP77svk.Data.Utils;

    [TestClass]
    public class OracleCredentialsParserParseTests
    {
        private readonly OracleCredentialsParser _parser = new OracleCredentialsParser();

        [TestMethod]
        public void ParseNullIsAllNulls()
        {
            _parser.Parse(null);
            Assert.IsNull(_parser.SchemaUser);
            Assert.IsNull(_parser.ProxyUser);
            Assert.IsNull(_parser.Password);
        }
    }
}
