namespace NoP77svk.Text.ConnectionStringParsers.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NoP77svk.Text.ConnectionStringParsers;

    [TestClass]
    public class ConnectionStringParserBuildTests
    {
        private readonly ConnectionStringParser<MockSubstringParser, MockSubstringParser> _parser
            = new ConnectionStringParser<MockSubstringParser, MockSubstringParser>(
                new MockSubstringParser(),
                new MockSubstringParser()
            );

        [TestMethod]
        public void TestDefaultUserServerDelimiter()
        {
            Assert.AreEqual("@", _parser.UserServerDelimiter);
        }

        [TestMethod]
        public void TestDefaultParsePriority()
        {
            Assert.AreEqual(ConnectionStringParserPartPriority.Server, _parser.ParsePriority);
        }

        [TestMethod]
        public void NullUserIsNullUser()
        {
            _parser.User = null;
            Assert.IsNull(_parser.User);
        }

        [TestMethod]
        public void EmptyUserIsEmptyUser()
        {
            _parser.User = string.Empty;
            Assert.AreEqual(string.Empty, _parser.User);
        }

        [TestMethod]
        public void EmptyServerIsEmptyServer()
        {
            _parser.Server = string.Empty;
            Assert.AreEqual(string.Empty, _parser.Server);
        }

        [TestMethod]
        public void NullServerIsNullServer()
        {
            _parser.Server = null;
            Assert.IsNull(_parser.Server);
        }

        [TestMethod]
        public void ConnectionStringWithBothNullsIsNull()
        {
            _parser.User = null;
            _parser.Server = null;
            Assert.IsNull(_parser.ConnectionString);
        }

        [TestMethod]
        public void ConnectionStringWithNullServerIsUserName()
        {
            _parser.User = "a_user";
            _parser.Server = null;
            Assert.AreEqual("a_user", _parser.ConnectionString);
        }

        [TestMethod]
        public void ConnectionStringWithEmptyServerIsUserName()
        {
            _parser.User = "a_user";
            _parser.Server = string.Empty;
            Assert.AreEqual("a_user@", _parser.ConnectionString);
        }

        [TestMethod]
        public void ConnectionStringWithNullUserIsServerName()
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
        public void ConnectionStringWithBothEmptyIsEmpty()
        {
            _parser.User = string.Empty;
            _parser.Server = string.Empty;
            Assert.AreEqual(string.Empty, _parser.ConnectionString);
        }

        [TestMethod]
        public void ConnectionStringFromUserNameAndServerHostBuildsOK()
        {
            _parser.User = "a_user";
            _parser.Server = "a_server";
            Assert.AreEqual("a_user@a_server", _parser.ConnectionString);
        }
    }
}
