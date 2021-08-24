namespace NoP77svk.Data.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NoP77svk.Data;

    [TestClass]
    public class ConnectionStringParserParseTests
    {
        private ConnectionStringParser _parser;

        public ConnectionStringParserParseTests() => _parser = new ConnectionStringParser();

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
            _parser = new ConnectionStringParser(parsePriority: ConnectionStringParserPartPriority.User)
            {
                ConnectionString = string.Empty
            };
            Assert.AreEqual(string.Empty, _parser.User);
            Assert.IsNull(_parser.Server);
        }

        [TestMethod]
        public void ParseEmptyIsEmptyServer()
        {
            _parser = new ConnectionStringParser(parsePriority: ConnectionStringParserPartPriority.Server)
            {
                ConnectionString = string.Empty
            };
            Assert.AreEqual(string.Empty, _parser.Server);
            Assert.IsNull(_parser.User);
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
            ConnectionStringParser localParser = new ConnectionStringParser(parsePriority: ConnectionStringParserPartPriority.User)
            {
                ConnectionString = "someone"
            };
            Assert.AreEqual("someone", localParser.User);
            Assert.IsNull(localParser.Server);
        }

        [TestMethod]
        public void ParseSingleValueOnlyIsServerWhenOverridden()
        {
            ConnectionStringParser localParser = new ConnectionStringParser(parsePriority: ConnectionStringParserPartPriority.Server)
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
    }
}