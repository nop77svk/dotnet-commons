namespace NoP77svk.Data.Utils.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NoP77svk.Data.Utils;

    [TestClass]
    public class OracleEzConnectStringParserParseTests
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
            var localParser = new ConnectionStringParser<GenericCredentialsParser, OracleServerStringParser>(
                new GenericCredentialsParser(),
                new OracleServerStringParser(),
                parsePriority: ConnectionStringParserPartPriority.User
            )
            {
                ConnectionString = string.Empty
            };
            Assert.AreEqual(string.Empty, localParser.User);
            Assert.IsNull(localParser.Server);
        }

        [TestMethod]
        public void ParseEmptyIsEmptyServer()
        {
            var localParser = new ConnectionStringParser<GenericCredentialsParser, OracleServerStringParser>(
                new GenericCredentialsParser(),
                new OracleServerStringParser(),
                parsePriority: ConnectionStringParserPartPriority.Server
            )
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
            var localParser = new ConnectionStringParser<GenericCredentialsParser, OracleServerStringParser>(
                new GenericCredentialsParser(),
                new OracleServerStringParser(),
                parsePriority: ConnectionStringParserPartPriority.User
            )
            {
                ConnectionString = "someone"
            };
            Assert.AreEqual("someone", localParser.User);
            Assert.IsNull(localParser.Server);
        }

        [TestMethod]
        public void ParseSingleValueOnlyIsServerWhenOverridden()
        {
            var localParser = new ConnectionStringParser<GenericCredentialsParser, OracleServerStringParser>(
                new GenericCredentialsParser(),
                new OracleServerStringParser(),
                parsePriority: ConnectionStringParserPartPriority.Server
            )
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
            _parser.ConnectionString = "a_user/a_password@a_server:1234/a_service";
            Assert.AreEqual("a_user", _parser.UserParser.Name);
            Assert.AreEqual("a_password", _parser.UserParser.Password);
            Assert.AreEqual("a_server", _parser.ServerParser.Host);
            Assert.AreEqual("1234", _parser.ServerParser.Port);
            Assert.AreEqual("a_service", _parser.ServerParser.ServiceOrSid);
        }

        [TestMethod]
        public void ParseWithEmptyServerPath()
        {
            _parser.ConnectionString = "a_user/a_password@a_server:1234/";
            Assert.AreEqual("a_user", _parser.UserParser.Name);
            Assert.AreEqual("a_password", _parser.UserParser.Password);
            Assert.AreEqual("a_server", _parser.ServerParser.Host);
            Assert.AreEqual("1234", _parser.ServerParser.Port);
            Assert.AreEqual(string.Empty, _parser.ServerParser.ServiceOrSid);
        }

        [TestMethod]
        public void ParseWithNoServerPath()
        {
            _parser.ConnectionString = "a_user/a_password@a_server:1234";
            Assert.AreEqual("a_user", _parser.UserParser.Name);
            Assert.AreEqual("a_password", _parser.UserParser.Password);
            Assert.AreEqual("a_server", _parser.ServerParser.Host);
            Assert.AreEqual("1234", _parser.ServerParser.Port);
            Assert.IsNull(_parser.ServerParser.ServiceOrSid);
        }

        [TestMethod]
        public void ParseWithEmptyServerPort()
        {
            _parser.ConnectionString = "a_user/a_password@a_server:/a_service";
            Assert.AreEqual("a_user", _parser.UserParser.Name);
            Assert.AreEqual("a_password", _parser.UserParser.Password);
            Assert.AreEqual("a_server", _parser.ServerParser.Host);
            Assert.AreEqual(string.Empty, _parser.ServerParser.Port);
            Assert.AreEqual("a_service", _parser.ServerParser.ServiceOrSid);
        }

        [TestMethod]
        public void ParseWithNullServerPort()
        {
            _parser.ConnectionString = "a_user/a_password@a_server/a_service";
            Assert.AreEqual("a_user", _parser.UserParser.Name);
            Assert.AreEqual("a_password", _parser.UserParser.Password);
            Assert.AreEqual("a_server", _parser.ServerParser.Host);
            Assert.IsNull(_parser.ServerParser.Port);
            Assert.AreEqual("a_service", _parser.ServerParser.ServiceOrSid);
        }

        [TestMethod]
        public void ParseWithEmptyServerHost()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                _parser.ConnectionString = "a_user/a_password@:1234/a_service";
            });
        }

        [TestMethod]
        public void ParseWithEmptyPassword()
        {
            _parser.ConnectionString = "a_user/@a_server:1234/a_service";
            Assert.AreEqual("a_user", _parser.UserParser.Name);
            Assert.AreEqual(string.Empty, _parser.UserParser.Password);
            Assert.AreEqual("a_server", _parser.ServerParser.Host);
            Assert.AreEqual("1234", _parser.ServerParser.Port);
            Assert.AreEqual("a_service", _parser.ServerParser.ServiceOrSid);
        }

        [TestMethod]
        public void ParseWithNullPassword()
        {
            _parser.ConnectionString = "a_user@a_server:1234/a_service";
            Assert.AreEqual("a_user", _parser.UserParser.Name);
            Assert.IsNull(_parser.UserParser.Password);
            Assert.AreEqual("a_server", _parser.ServerParser.Host);
            Assert.AreEqual("1234", _parser.ServerParser.Port);
            Assert.AreEqual("a_service", _parser.ServerParser.ServiceOrSid);
        }

        [TestMethod]
        public void ParseWithEmptyUserName()
        {
            _parser.ConnectionString = "/a_password@a_server:1234/a_service";
            Assert.AreEqual(string.Empty, _parser.UserParser.Name);
            Assert.AreEqual("a_password", _parser.UserParser.Password);
            Assert.AreEqual("a_server", _parser.ServerParser.Host);
            Assert.AreEqual("1234", _parser.ServerParser.Port);
            Assert.AreEqual("a_service", _parser.ServerParser.ServiceOrSid);
        }
    }
}
