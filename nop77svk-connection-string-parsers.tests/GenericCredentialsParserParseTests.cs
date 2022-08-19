namespace NoP77svk.Text.ConnectionStringParsers.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NoP77svk.Text.ConnectionStringParsers;

    [TestClass]
    public class GenericCredentialsParserParseTests
    {
        private readonly GenericCredentialsParser _parser = new GenericCredentialsParser();

        [TestMethod]
        public void ParseNullIsBothNulls()
        {
            _parser.Parse(null);
            Assert.IsNull(_parser.Name);
            Assert.IsNull(_parser.Password);
        }

        [TestMethod]
        public void ParseEmptyIsEmptyName()
        {
            _parser.Parse(string.Empty);
            Assert.AreEqual(string.Empty, _parser.Name);
            Assert.IsNull(_parser.Password);
        }

        [TestMethod]
        public void ParseDelimiterIsBothEmpty()
        {
            _parser.Parse("/");
            Assert.AreEqual(string.Empty, _parser.Name);
            Assert.AreEqual(string.Empty, _parser.Password);
        }

        [TestMethod]
        public void ParseNameIsNameAndNullPassword()
        {
            _parser.Parse("a_user");
            Assert.AreEqual("a_user", _parser.Name);
            Assert.IsNull(_parser.Password);
        }

        [TestMethod]
        public void ParseNameAndDelimiterIsNameAndEmptyPassword()
        {
            _parser.Parse("a_user/");
            Assert.AreEqual("a_user", _parser.Name);
            Assert.AreEqual(string.Empty, _parser.Password);
        }

        [TestMethod]
        public void ParseEmptyNameAndFilledPasswordIsEmptyNameAndPassword()
        {
            _parser.Parse("/a_password");
            Assert.AreEqual(string.Empty, _parser.Name);
            Assert.AreEqual("a_password", _parser.Password);
        }

        [TestMethod]
        public void ParseOK()
        {
            _parser.Parse("a_user/a_password");
            Assert.AreEqual("a_user", _parser.Name);
            Assert.AreEqual("a_password", _parser.Password);
        }
    }
}