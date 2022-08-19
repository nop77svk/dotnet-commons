namespace NoP77svk.Text.ConnectionStringParsers.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NoP77svk.Text.ConnectionStringParsers;

    [TestClass]
    public class OracleEzConnectStringParserBuildTests
    {
        private readonly ConnectionStringParser<OracleCredentialsParser, OracleServerStringParser> _parser
            = new ConnectionStringParser<OracleCredentialsParser, OracleServerStringParser>(
                new OracleCredentialsParser(),
                new OracleServerStringParser()
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
        public void TestDefaultNamePasswordDelimiter()
        {
            Assert.AreEqual("/", _parser.UserParser.NamePasswordDelimiter);
        }

        [TestMethod]
        public void TestDefaultHostPortDelimiter()
        {
            Assert.AreEqual(":", _parser.ServerParser.HostPortDelimiter);
        }

        [TestMethod]
        public void TestDefaultServiceOrSidDelimiter()
        {
            Assert.AreEqual("/", _parser.ServerParser.ServiceOrSidDelimiter);
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

        [TestMethod]
        public void ConnectionStringFromFullServerSpecBuildsOK()
        {
            _parser.UserParser.Name = "a_user";
            _parser.UserParser.Password = "a_heslo";
            _parser.ServerParser.Host = "a_server";
            _parser.ServerParser.Port = "1234";
            _parser.ServerParser.ServiceOrSid = "a_service";
            Assert.AreEqual("a_user/a_heslo@a_server:1234/a_service", _parser.ConnectionString);
        }

        [TestMethod]
        public void ConnectionStringNullPathBuildsOK()
        {
            _parser.UserParser.Name = "a_user";
            _parser.UserParser.Password = "a_heslo";
            _parser.ServerParser.Host = "a_server";
            _parser.ServerParser.Port = "1234";
            _parser.ServerParser.ServiceOrSid = null;
            Assert.AreEqual("a_user/a_heslo@a_server:1234", _parser.ConnectionString);
        }

        [TestMethod]
        public void ConnectionStringEmptyPathBuildsOK()
        {
            _parser.UserParser.Name = "a_user";
            _parser.UserParser.Password = "a_heslo";
            _parser.ServerParser.Host = "a_server";
            _parser.ServerParser.Port = "1234";
            _parser.ServerParser.ServiceOrSid = string.Empty;
            Assert.AreEqual("a_user/a_heslo@a_server:1234/", _parser.ConnectionString);
        }

        [TestMethod]
        public void ConnectionStringNullPortBuildsOK()
        {
            _parser.UserParser.Name = "a_user";
            _parser.UserParser.Password = "a_heslo";
            _parser.ServerParser.Host = "a_server";
            _parser.ServerParser.Port = null;
            _parser.ServerParser.ServiceOrSid = "a_service";
            Assert.AreEqual("a_user/a_heslo@a_server/a_service", _parser.ConnectionString);
        }

        [TestMethod]
        public void ConnectionStringEmptyPortBuildsOK()
        {
            _parser.UserParser.Name = "a_user";
            _parser.UserParser.Password = "a_heslo";
            _parser.ServerParser.Host = "a_server";
            _parser.ServerParser.Port = string.Empty;
            _parser.ServerParser.ServiceOrSid = "a_service";
            Assert.AreEqual("a_user/a_heslo@a_server:/a_service", _parser.ConnectionString);
        }

        [TestMethod]
        public void ConnectionStringNullHostIsUser()
        {
            _parser.UserParser.Name = "a_user";
            _parser.UserParser.Password = "a_heslo";
            _parser.ServerParser.Host = null;
            _parser.ServerParser.Port = "1234";
            _parser.ServerParser.ServiceOrSid = "a_service";
            Assert.AreEqual("a_user/a_heslo", _parser.ConnectionString);
        }

        [TestMethod]
        public void ConnectionStringEmptyHostBuildsOK()
        {
            _parser.UserParser.Name = "a_user";
            _parser.UserParser.Password = "a_heslo";
            _parser.ServerParser.Host = string.Empty;
            _parser.ServerParser.Port = "1234";
            _parser.ServerParser.ServiceOrSid = "a_service";
            Assert.AreEqual("a_user/a_heslo@:1234/a_service", _parser.ConnectionString);
        }

        [TestMethod]
        public void ConnectionStringNullUserNameIsNull()
        {
            _parser.UserParser.Name = null;
            _parser.UserParser.Password = "a_heslo";
            _parser.ServerParser.Host = "a_server";
            _parser.ServerParser.Port = "1234";
            _parser.ServerParser.ServiceOrSid = "a_service";
            Assert.IsNull(_parser.ConnectionString);
        }

        [TestMethod]
        public void ConnectionStringEmptyUserNameBuildsOK()
        {
            _parser.UserParser.Name = string.Empty;
            _parser.UserParser.Password = "a_heslo";
            _parser.ServerParser.Host = "a_server";
            _parser.ServerParser.Port = "1234";
            _parser.ServerParser.ServiceOrSid = "a_service";
            Assert.AreEqual("/a_heslo@a_server:1234/a_service", _parser.ConnectionString);
        }

        [TestMethod]
        public void ConnectionStringNullUserPasswordBuildsOK()
        {
            _parser.UserParser.Name = "a_user";
            _parser.UserParser.Password = null;
            _parser.ServerParser.Host = "a_server";
            _parser.ServerParser.Port = "1234";
            _parser.ServerParser.ServiceOrSid = "a_service";
            Assert.AreEqual("a_user@a_server:1234/a_service", _parser.ConnectionString);
        }

        [TestMethod]
        public void ConnectionStringEmptyUserPasswordBuildsOK()
        {
            _parser.UserParser.Name = "a_user";
            _parser.UserParser.Password = string.Empty;
            _parser.ServerParser.Host = "a_server";
            _parser.ServerParser.Port = "1234";
            _parser.ServerParser.ServiceOrSid = "a_service";
            Assert.AreEqual("a_user/@a_server:1234/a_service", _parser.ConnectionString);
        }
    }
}
