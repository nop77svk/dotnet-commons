namespace NoP77svk.Data.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NoP77svk.Data.Utils;

    [TestClass]
    public class ConnectionStringParserBuildTests
    {
        private readonly ConnectionStringParser _parser;

        public ConnectionStringParserBuildTests() => _parser = new ConnectionStringParser();

        [TestMethod]
        public void TestDefaultParserUserServerDelimiter()
        {
            Assert.AreEqual("@", _parser.UserServerDelimiter);
        }

        public void TestDefaultParserParsePriority()
        {
            Assert.AreEqual(ConnectionStringParserPartPriority.User, _parser.ParsePriority);
        }

        [TestMethod]
        public void ConnectionStringWithBothNullsIsNull()
        {
            _parser.User = null;
            _parser.Server = null;
            Assert.IsNull(_parser.ConnectionString);
        }

        [TestMethod]
        public void ConnectionStringWithBothEmptyIsEmpty()
        {
            _parser.User = string.Empty;
            _parser.Server = string.Empty;
            Assert.AreEqual(string.Empty, _parser.ConnectionString);
        }

        [TestMethod]
        public void ConnectionStringWithNullServerIsUserName()
        {
            _parser.User = "a_user";
            _parser.Server = null;
            Assert.AreEqual("a_user", _parser.ConnectionString);
        }

        [TestMethod]
        public void ConnectionStringWithEmptyServerIsUserNameAt()
        {
            _parser.User = "a_user";
            _parser.Server = string.Empty;
            Assert.AreEqual("a_user@", _parser.ConnectionString);
        }

        [TestMethod]
        public void ConnectionStringWithNullUserIsNull()
        {
            _parser.User = null;
            _parser.Server = "a_server";
            Assert.IsNull(_parser.ConnectionString);
        }

        [TestMethod]
        public void ConnectionStringWithEmptyUserIsAtServerName()
        {
            _parser.User = string.Empty;
            _parser.Server = "a_server";
            Assert.AreEqual("@a_server", _parser.ConnectionString);
        }

        [TestMethod]
        public void ConnectionStringBuildsOK()
        {
            _parser.User = "a_user";
            _parser.Server = "a_server";
            Assert.AreEqual("a_user@a_server", _parser.ConnectionString);
        }
    }
}