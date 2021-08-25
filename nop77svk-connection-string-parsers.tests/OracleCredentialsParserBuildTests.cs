namespace NoP77svk.Data.Utils.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NoP77svk.Data.Utils;

    [TestClass]
    public class OracleCredentialsParserBuildTests
    {
        private readonly OracleCredentialsParser _parser = new OracleCredentialsParser();

        [TestMethod]
        public void AllNullBuildsNull()
        {
            _parser.SchemaUser = null;
            _parser.ProxyUser = null;
            _parser.Password = null;
            Assert.IsNull(_parser.Build());
        }
    }
}
