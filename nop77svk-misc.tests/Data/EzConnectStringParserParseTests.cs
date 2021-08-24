namespace NoP77svk.Data.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NoP77svk.Data.Utils;

    [TestClass]
    public class EzConnectStringParserParseTests
    {
        private readonly EzConnectStringParser _parser;

        public EzConnectStringParserParseTests() => _parser = new EzConnectStringParser();

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
        public void ParseNullIsNullUser()
        {
            _parser.ConnectionString = null;
            Assert.IsNull(_parser.User);
        }

        [TestMethod]
        public void ParseNullIsNullServer()
        {
            _parser.ConnectionString = null;
            Assert.IsNull(_parser.Server);
        }

        [TestMethod]
        public void ParseEmptyIsEmptyUser()
        {
            EzConnectStringParser localParser = new EzConnectStringParser(parsePriority: ConnectionStringParserPartPriority.User)
            {
                ConnectionString = string.Empty
            };
            Assert.AreEqual(string.Empty, localParser.User);
            Assert.IsNull(localParser.Server);
        }

        [TestMethod]
        public void ParseEmptyIsEmptyServer()
        {
            EzConnectStringParser localParser = new EzConnectStringParser(parsePriority: ConnectionStringParserPartPriority.Server)
            {
                ConnectionString = string.Empty
            };
            Assert.AreEqual(string.Empty, localParser.Server);
            Assert.IsNull(localParser.User);
        }

        [TestMethod]
        public void ParseSingleValueOnlyIsServerByDefault()
        {
            _parser.ConnectionString = "something";
            Assert.AreEqual("something", _parser.Server);
            Assert.IsNull(_parser.User);
        }

        [TestMethod]
        public void ParseSingleValueOnlyIsUserWhenOverridden()
        {
            EzConnectStringParser localParser = new EzConnectStringParser(parsePriority: ConnectionStringParserPartPriority.User)
            {
                ConnectionString = "someone"
            };
            Assert.AreEqual("someone", localParser.User);
            Assert.IsNull(localParser.Server);
        }

        [TestMethod]
        public void ParseSingleValueOnlyIsServerWhenOverridden()
        {
            EzConnectStringParser localParser = new EzConnectStringParser(parsePriority: ConnectionStringParserPartPriority.Server)
            {
                ConnectionString = "something"
            };
            Assert.AreEqual("something", localParser.Server);
            Assert.IsNull(localParser.User);
        }

        [TestMethod]
        public void ParseBothIsOK()
        {
            _parser.ConnectionString = "someone@something";
            Assert.AreEqual("someone", _parser.User);
            Assert.AreEqual("something", _parser.Server);
        }

        [TestMethod]
        public void FullStringParsesOK()
        {
            _parser.ConnectionString = "a_user/a_password@a_server:1234/folder1/folder2/folder3";
            Assert.AreEqual("a_user", _parser.UserName);
            Assert.AreEqual("a_password", _parser.UserPassword);
            Assert.AreEqual("a_server", _parser.ServerHost);
            Assert.AreEqual("1234", _parser.ServerPort);
            Assert.AreEqual("folder1/folder2/folder3", _parser.ServerPath);
        }

        [TestMethod]
        public void ParseWithEmptyServerPath()
        {
            _parser.ConnectionString = "a_user/a_password@a_server:1234/";
            Assert.AreEqual("a_user", _parser.UserName);
            Assert.AreEqual("a_password", _parser.UserPassword);
            Assert.AreEqual("a_server", _parser.ServerHost);
            Assert.AreEqual("1234", _parser.ServerPort);
            Assert.AreEqual(string.Empty, _parser.ServerPath);
        }

        [TestMethod]
        public void ParseWithNoServerPath()
        {
            _parser.ConnectionString = "a_user/a_password@a_server:1234";
            Assert.AreEqual("a_user", _parser.UserName);
            Assert.AreEqual("a_password", _parser.UserPassword);
            Assert.AreEqual("a_server", _parser.ServerHost);
            Assert.AreEqual("1234", _parser.ServerPort);
            Assert.IsNull(_parser.ServerPath);
        }

        [TestMethod]
        public void ParseWithEmptyServerPort()
        {
            _parser.ConnectionString = "a_user/a_password@a_server:/folder1/folder2/folder3";
            Assert.AreEqual("a_user", _parser.UserName);
            Assert.AreEqual("a_password", _parser.UserPassword);
            Assert.AreEqual("a_server", _parser.ServerHost);
            Assert.AreEqual(string.Empty, _parser.ServerPort);
            Assert.AreEqual("folder1/folder2/folder3", _parser.ServerPath);
        }

        [TestMethod]
        public void ParseWithNullServerPort()
        {
            _parser.ConnectionString = "a_user/a_password@a_server/folder1/folder2/folder3";
            Assert.AreEqual("a_user", _parser.UserName);
            Assert.AreEqual("a_password", _parser.UserPassword);
            Assert.AreEqual("a_server", _parser.ServerHost);
            Assert.IsNull(_parser.ServerPort);
            Assert.AreEqual("folder1/folder2/folder3", _parser.ServerPath);
        }

        [TestMethod]
        public void ParseWithEmptyServerHost()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                _parser.ConnectionString = "a_user/a_password@:1234/folder1/folder2/folder3";
            });
        }

        [TestMethod]
        public void ParseWithEmptyPassword()
        {
            _parser.ConnectionString = "a_user/@a_server:1234/folder1/folder2/folder3";
            Assert.AreEqual("a_user", _parser.UserName);
            Assert.AreEqual(string.Empty, _parser.UserPassword);
            Assert.AreEqual("a_server", _parser.ServerHost);
            Assert.AreEqual("1234", _parser.ServerPort);
            Assert.AreEqual("folder1/folder2/folder3", _parser.ServerPath);
        }

        [TestMethod]
        public void ParseWithNullPassword()
        {
            _parser.ConnectionString = "a_user@a_server:1234/folder1/folder2/folder3";
            Assert.AreEqual("a_user", _parser.UserName);
            Assert.IsNull(_parser.UserPassword);
            Assert.AreEqual("a_server", _parser.ServerHost);
            Assert.AreEqual("1234", _parser.ServerPort);
            Assert.AreEqual("folder1/folder2/folder3", _parser.ServerPath);
        }

        [TestMethod]
        public void ParseWithEmptyUserName()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                _parser.ConnectionString = "/a_password@a_server:1234/folder1/folder2/folder3";
            });
        }
    }
}
