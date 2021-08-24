namespace NoP77svk.Data.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NoP77svk.Data.Utils;

    [TestClass]
    public class EzConnectStringParserBuildTests
    {
        private readonly EzConnectStringParser _parser;

        public EzConnectStringParserBuildTests()
        {
            _parser = new EzConnectStringParser();
        }

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
            Assert.AreEqual("/", _parser.NamePasswordDelimiter);
        }

        [TestMethod]
        public void TestDefaultHostPortDelimiter()
        {
            Assert.AreEqual(":", _parser.HostPortDelimiter);
        }

        [TestMethod]
        public void TestDefaultHostPathDelimiter()
        {
            Assert.AreEqual("/", _parser.HostPathDelimiter);
        }

        [TestMethod]
        public void NullUserIsNullUser()
        {
            _parser.User = null;
            Assert.IsNull(_parser.User);
        }

        [TestMethod]
        public void EmptyServerIsEmptyServer()
        {
            _parser.Server = string.Empty;
            Assert.AreEqual(string.Empty, _parser.Server);
        }

        [TestMethod]
        public void EmptyUserIsEmptyUser()
        {
            _parser.User = string.Empty;
            Assert.AreEqual(string.Empty, _parser.User);
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
            _parser.UserName = "a_user";
            _parser.UserPassword = "a_heslo";
            _parser.ServerHost = "a_server";
            _parser.ServerPort = "1234";
            _parser.ServerPath = "shared/physical";
            Assert.AreEqual("a_user/a_heslo@a_server:1234/shared/physical", _parser.ConnectionString);
        }

        [TestMethod]
        public void ConnectionStringNullPathBuildsOK()
        {
            _parser.UserName = "a_user";
            _parser.UserPassword = "a_heslo";
            _parser.ServerHost = "a_server";
            _parser.ServerPort = "1234";
            _parser.ServerPath = null;
            Assert.AreEqual("a_user/a_heslo@a_server:1234", _parser.ConnectionString);
        }

        [TestMethod]
        public void ConnectionStringEmptyPathBuildsOK()
        {
            _parser.UserName = "a_user";
            _parser.UserPassword = "a_heslo";
            _parser.ServerHost = "a_server";
            _parser.ServerPort = "1234";
            _parser.ServerPath = string.Empty;
            Assert.AreEqual("a_user/a_heslo@a_server:1234/", _parser.ConnectionString);
        }

        [TestMethod]
        public void ConnectionStringNullPortBuildsOK()
        {
            _parser.UserName = "a_user";
            _parser.UserPassword = "a_heslo";
            _parser.ServerHost = "a_server";
            _parser.ServerPort = null;
            _parser.ServerPath = "shared/physical";
            Assert.AreEqual("a_user/a_heslo@a_server/shared/physical", _parser.ConnectionString);
        }

        [TestMethod]
        public void ConnectionStringEmptyPortBuildsOK()
        {
            _parser.UserName = "a_user";
            _parser.UserPassword = "a_heslo";
            _parser.ServerHost = "a_server";
            _parser.ServerPort = string.Empty;
            _parser.ServerPath = "shared/physical";
            Assert.AreEqual("a_user/a_heslo@a_server:/shared/physical", _parser.ConnectionString);
        }

        [TestMethod]
        public void ConnectionStringNullHostIsUser()
        {
            _parser.UserName = "a_user";
            _parser.UserPassword = "a_heslo";
            _parser.ServerHost = null;
            _parser.ServerPort = "1234";
            _parser.ServerPath = "shared/physical";
            Assert.AreEqual("a_user/a_heslo", _parser.ConnectionString);
        }

        [TestMethod]
        public void ConnectionStringEmptyHostBuildsOK()
        {
            _parser.UserName = "a_user";
            _parser.UserPassword = "a_heslo";
            _parser.ServerHost = string.Empty;
            _parser.ServerPort = "1234";
            _parser.ServerPath = "shared/physical";
            Assert.AreEqual("a_user/a_heslo@:1234/shared/physical", _parser.ConnectionString);
        }

        [TestMethod]
        public void ConnectionStringNullUserNameIsNull()
        {
            _parser.UserName = null;
            _parser.UserPassword = "a_heslo";
            _parser.ServerHost = "a_server";
            _parser.ServerPort = "1234";
            _parser.ServerPath = "shared/physical";
            Assert.IsNull(_parser.ConnectionString);
        }

        [TestMethod]
        public void ConnectionStringEmptyUserNameBuildsOK()
        {
            _parser.UserName = string.Empty;
            _parser.UserPassword = "a_heslo";
            _parser.ServerHost = "a_server";
            _parser.ServerPort = "1234";
            _parser.ServerPath = "shared/physical";
            Assert.AreEqual("/a_heslo@a_server:1234/shared/physical", _parser.ConnectionString);
        }

        [TestMethod]
        public void ConnectionStringNullUserPasswordBuildsOK()
        {
            _parser.UserName = "a_user";
            _parser.UserPassword = null;
            _parser.ServerHost = "a_server";
            _parser.ServerPort = "1234";
            _parser.ServerPath = "shared/physical";
            Assert.AreEqual("a_user@a_server:1234/shared/physical", _parser.ConnectionString);
        }

        [TestMethod]
        public void ConnectionStringEmptyUserPasswordBuildsOK()
        {
            _parser.UserName = "a_user";
            _parser.UserPassword = string.Empty;
            _parser.ServerHost = "a_server";
            _parser.ServerPort = "1234";
            _parser.ServerPath = "shared/physical";
            Assert.AreEqual("a_user/@a_server:1234/shared/physical", _parser.ConnectionString);
        }
    }
}
