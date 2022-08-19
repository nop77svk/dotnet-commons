namespace NoP77svk.Text.ConnectionStringParsers.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NoP77svk.Text.ConnectionStringParsers;

    [TestClass]
    public class GenericCredentialsParserBuildTests
    {
        private readonly GenericCredentialsParser _parser = new GenericCredentialsParser();

        [TestMethod]
        public void BothNullBuildsNull()
        {
            _parser.Name = null;
            _parser.Password = null;
            Assert.IsNull(_parser.Build());
        }

        [TestMethod]
        public void EmptyNameAndNullPasswordIsEmpty()
        {
            _parser.Name = string.Empty;
            _parser.Password = null;
            Assert.AreEqual(string.Empty, _parser.Build());
        }

        [TestMethod]
        public void NullNameAndEmptyPasswordIsNull()
        {
            _parser.Name = null;
            _parser.Password = string.Empty;
            Assert.IsNull(_parser.Build());
        }

        [TestMethod]
        public void EmptyNameAndEmptyPasswordIsDelimiter()
        {
            _parser.Name = string.Empty;
            _parser.Password = string.Empty;
            Assert.AreEqual("/", _parser.Build());
        }

        [TestMethod]
        public void NameAndNullPasswordIsName()
        {
            _parser.Name = "a_user";
            _parser.Password = null;
            Assert.AreEqual("a_user", _parser.Build());
        }

        [TestMethod]
        public void NameAndEmptyPasswordIsNameAndDelimiter()
        {
            _parser.Name = "a_user";
            _parser.Password = string.Empty;
            Assert.AreEqual("a_user/", _parser.Build());
        }

        [TestMethod]
        public void NullNameAndPasswordIsNull()
        {
            _parser.Name = null;
            _parser.Password = "a_password";
            Assert.IsNull(_parser.Build());
        }

        [TestMethod]
        public void EmptyNameAndPasswordIsSlashPassword()
        {
            _parser.Name = string.Empty;
            _parser.Password = "a_password";
            Assert.AreEqual("/a_password", _parser.Build());
        }

        [TestMethod]
        public void NameAndPasswordBuildsOK()
        {
            _parser.Name = "a_user";
            _parser.Password = "a_password";
            Assert.AreEqual("a_user/a_password", _parser.Build());
        }
    }
}