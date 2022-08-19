namespace NoP77svk.Text.ConnectionStringParsers.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NoP77svk.Text.ConnectionStringParsers;

    [TestClass]
    public class OracleCredentialsParserParseTests
    {
        private readonly OracleCredentialsParser _parser = new OracleCredentialsParser();

        [TestMethod]
        public void NullParsesSchemaNullProxyNullPasswordNull()
        {
            _parser.Parse(null);
            Assert.AreEqual(null, _parser.SchemaUser);
            Assert.AreEqual(null, _parser.ProxyUser);
            Assert.AreEqual(null, _parser.Password);
        }

        [TestMethod]
        public void EmptyParsesSchemaEmptyProxyNullPasswordNull()
        {
            _parser.Parse(string.Empty);
            Assert.AreEqual(string.Empty, _parser.SchemaUser);
            Assert.AreEqual(null, _parser.ProxyUser);
            Assert.AreEqual(null, _parser.Password);
        }

        [TestMethod]
        public void SlashParsesSchemaEmptyProxyNullPasswordEmpty()
        {
            _parser.Parse("/");
            Assert.AreEqual(string.Empty, _parser.SchemaUser);
            Assert.AreEqual(null, _parser.ProxyUser);
            Assert.AreEqual(string.Empty, _parser.Password);
        }

        [TestMethod]
        public void SlashPasswordParsesSchemaEmptyProxyNullPasswordFilled()
        {
            _parser.Parse("/a_password");
            Assert.AreEqual(string.Empty, _parser.SchemaUser);
            Assert.AreEqual(null, _parser.ProxyUser);
            Assert.AreEqual("a_password", _parser.Password);
        }

        [TestMethod]
        public void BracketsParsesSchemaEmptyProxyEmptyPasswordNull()
        {
            _parser.Parse("[]");
            Assert.AreEqual(string.Empty, _parser.SchemaUser);
            Assert.AreEqual(string.Empty, _parser.ProxyUser);
            Assert.AreEqual(null, _parser.Password);
        }

        [TestMethod]
        public void BracketsAndSlashParsesSchemaEmptyProxyEmptyPasswordEmpty()
        {
            _parser.Parse("[]/");
            Assert.AreEqual(string.Empty, _parser.SchemaUser);
            Assert.AreEqual(string.Empty, _parser.ProxyUser);
            Assert.AreEqual(string.Empty, _parser.Password);
        }

        [TestMethod]
        public void BracketsAndPasswordParsesSchemaEmptyProxyEmptyPasswordFilled()
        {
            _parser.Parse("[]/a_password");
            Assert.AreEqual(string.Empty, _parser.SchemaUser);
            Assert.AreEqual(string.Empty, _parser.ProxyUser);
            Assert.AreEqual("a_password", _parser.Password);
        }

        [TestMethod]
        public void ProxyAndBracketsParsesSchemaEmptyProxyFilledPasswordNull()
        {
            _parser.Parse("a_proxy[]");
            Assert.AreEqual(string.Empty, _parser.SchemaUser);
            Assert.AreEqual("a_proxy", _parser.ProxyUser);
            Assert.AreEqual(null, _parser.Password);
        }

        [TestMethod]
        public void ProxyBracketsAndSlashParsesSchemaEmptyProxyFilledPasswordEmpty()
        {
            _parser.Parse("a_proxy[]/");
            Assert.AreEqual(string.Empty, _parser.SchemaUser);
            Assert.AreEqual("a_proxy", _parser.ProxyUser);
            Assert.AreEqual(string.Empty, _parser.Password);
        }

        [TestMethod]
        public void ProxyBracketsAndPasswordParsesSchemaEmptyProxyFilledPasswordFilled()
        {
            _parser.Parse("a_proxy[]/a_password");
            Assert.AreEqual(string.Empty, _parser.SchemaUser);
            Assert.AreEqual("a_proxy", _parser.ProxyUser);
            Assert.AreEqual("a_password", _parser.Password);
        }

        [TestMethod]
        public void UserParsesSchemaFilledProxyNullPasswordNull()
        {
            _parser.Parse("a_user");
            Assert.AreEqual("a_user", _parser.SchemaUser);
            Assert.AreEqual(null, _parser.ProxyUser);
            Assert.AreEqual(null, _parser.Password);
        }

        [TestMethod]
        public void UserAndSlashParsesSchemaFilledProxyNullPasswordEmpty()
        {
            _parser.Parse("a_user/");
            Assert.AreEqual("a_user", _parser.SchemaUser);
            Assert.AreEqual(null, _parser.ProxyUser);
            Assert.AreEqual(string.Empty, _parser.Password);
        }

        [TestMethod]
        public void UserAndPasswordParsesSchemaFilledProxyNullPasswordFilled()
        {
            _parser.Parse("a_user/a_password");
            Assert.AreEqual("a_user", _parser.SchemaUser);
            Assert.AreEqual(null, _parser.ProxyUser);
            Assert.AreEqual("a_password", _parser.Password);
        }

        [TestMethod]
        public void BracketedUserParsesSchemaFilledProxyEmptyPasswordNull()
        {
            _parser.Parse("[a_user]");
            Assert.AreEqual("a_user", _parser.SchemaUser);
            Assert.AreEqual(string.Empty, _parser.ProxyUser);
            Assert.AreEqual(null, _parser.Password);
        }

        [TestMethod]
        public void BracketedUserAndSlashParsesSchemaFilledProxyEmptyPasswordEmpty()
        {
            _parser.Parse("[a_user]/");
            Assert.AreEqual("a_user", _parser.SchemaUser);
            Assert.AreEqual(string.Empty, _parser.ProxyUser);
            Assert.AreEqual(string.Empty, _parser.Password);
        }

        [TestMethod]
        public void BracketedUserAndPasswordParsesSchemaFilledProxyEmptyPasswordFilled()
        {
            _parser.Parse("[a_user]/a_password");
            Assert.AreEqual("a_user", _parser.SchemaUser);
            Assert.AreEqual(string.Empty, _parser.ProxyUser);
            Assert.AreEqual("a_password", _parser.Password);
        }

        [TestMethod]
        public void ProxiedUserParsesSchemaFilledProxyFilledPasswordNull()
        {
            _parser.Parse("a_proxy[a_user]");
            Assert.AreEqual("a_user", _parser.SchemaUser);
            Assert.AreEqual("a_proxy", _parser.ProxyUser);
            Assert.AreEqual(null, _parser.Password);
        }

        [TestMethod]
        public void ProxiedUserAndSlashParsesSchemaFilledProxyFilledPasswordEmpty()
        {
            _parser.Parse("a_proxy[a_user]/");
            Assert.AreEqual("a_user", _parser.SchemaUser);
            Assert.AreEqual("a_proxy", _parser.ProxyUser);
            Assert.AreEqual(string.Empty, _parser.Password);
        }

        [TestMethod]
        public void OkParsesSchemaFilledProxyFilledPasswordFilled()
        {
            _parser.Parse("a_proxy[a_user]/a_password");
            Assert.AreEqual("a_user", _parser.SchemaUser);
            Assert.AreEqual("a_proxy", _parser.ProxyUser);
            Assert.AreEqual("a_password", _parser.Password);
        }
    }
}
