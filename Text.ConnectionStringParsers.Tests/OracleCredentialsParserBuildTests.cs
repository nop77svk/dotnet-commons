﻿namespace NoP77svk.Text.ConnectionStringParsers.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NoP77svk.Text.ConnectionStringParsers;

    /* Generated by
    with xyz as (
        select '
                    [TestMethod]
                    public void Schema'||t_user.column_value||'Proxy'||t_proxy.column_value||'Password'||t_password.column_value||'Builds__'||lpad(rownum,3,'0')||'()
                    {
                        _parser.SchemaUser = '||decode(t_user.column_value, 'Null','null', 'Empty','string.Empty', 'Filled','"a_user"')||';
                        _parser.ProxyUser = '||decode(t_proxy.column_value, 'Null','null', 'Empty','string.Empty', 'Filled','"a_proxy"')||';
                        _parser.Password = '||decode(t_password.column_value, 'Null','null', 'Empty','string.Empty', 'Filled','"a_password"')||';
                        Assert.AreEqual("...", _parser.Build());
                        Assert.IsNull(_parser.Build());
                    }'
                as cs_build_ut,
            '
                    [TestMethod]
                    public void X'||lpad(rownum,3,0)||'___ParsesTo'||t_user.column_value||'Schema'||t_proxy.column_value||'Proxy'||t_password.column_value||'Password'||'
                    {
                        _parser.Parse("...");
                        Assert.AreEqual('||decode(t_user.column_value, 'Null','null', 'Empty','string.Empty', 'Filled','"a_user"')||', _parser.SchemaUser);
                        Assert.AreEqual('||decode(t_proxy.column_value, 'Null','null', 'Empty','string.Empty', 'Filled','"a_user"')||', _parser.ProxyUser);
                        Assert.AreEqual('||decode(t_password.column_value, 'Null','null', 'Empty','string.Empty', 'Filled','"a_user"')||', _parser.Password);
                    }'
                as cs_parse_ut
        from table(arr_varchar2_4000('Null','Empty','Filled')) t_user
            cross join table(arr_varchar2_4000('Null','Empty','Filled')) t_proxy
            cross join table(arr_varchar2_4000('Null','Empty','Filled')) t_password
    )
    select
        phr_clob
            .new_tmp_lob()
            .append(
                i_values => cursor(select cs_build_ut from xyz),
                i_delimiter => chr(10)
            )
            .get_value()
            as test_suite_build,
        phr_clob
            .new_tmp_lob()
            .append(
                i_values => cursor(select cs_parse_ut from xyz),
                i_delimiter => chr(10)
            )
            .get_value()
            as test_suite_parse
    from dual
    ;
    */

    [TestClass]
    public class OracleCredentialsParserBuildTests
    {
        private readonly OracleCredentialsParser _parser = new OracleCredentialsParser();

        [TestMethod]
        public void SchemaNullProxyNullPasswordNullBuildsNull()
        {
            _parser.SchemaUser = null;
            _parser.ProxyUser = null;
            _parser.Password = null;
            Assert.IsNull(_parser.Build());
        }

        [TestMethod]
        public void SchemaNullProxyNullPasswordEmptyBuildsNull()
        {
            _parser.SchemaUser = null;
            _parser.ProxyUser = null;
            _parser.Password = string.Empty;
            Assert.IsNull(_parser.Build());
        }

        [TestMethod]
        public void SchemaNullProxyNullPasswordFilledBuildsNull()
        {
            _parser.SchemaUser = null;
            _parser.ProxyUser = null;
            _parser.Password = "a_password";
            Assert.IsNull(_parser.Build());
        }

        [TestMethod]
        public void SchemaNullProxyEmptyPasswordNullBuildsNull()
        {
            _parser.SchemaUser = null;
            _parser.ProxyUser = string.Empty;
            _parser.Password = null;
            Assert.IsNull(_parser.Build());
        }

        [TestMethod]
        public void SchemaNullProxyEmptyPasswordEmptyBuildsNull()
        {
            _parser.SchemaUser = null;
            _parser.ProxyUser = string.Empty;
            _parser.Password = string.Empty;
            Assert.IsNull(_parser.Build());
        }

        [TestMethod]
        public void SchemaNullProxyEmptyPasswordFilledBuildsNull()
        {
            _parser.SchemaUser = null;
            _parser.ProxyUser = string.Empty;
            _parser.Password = "a_password";
            Assert.IsNull(_parser.Build());
        }

        [TestMethod]
        public void SchemaNullProxyFilledPasswordNullBuildsNull()
        {
            _parser.SchemaUser = null;
            _parser.ProxyUser = "a_proxy";
            _parser.Password = null;
            Assert.IsNull(_parser.Build());
        }

        [TestMethod]
        public void SchemaNullProxyFilledPasswordEmptyBuildsNull()
        {
            _parser.SchemaUser = null;
            _parser.ProxyUser = "a_proxy";
            _parser.Password = string.Empty;
            Assert.IsNull(_parser.Build());
        }

        [TestMethod]
        public void SchemaNullProxyFilledPasswordFilledBuildsNull()
        {
            _parser.SchemaUser = null;
            _parser.ProxyUser = "a_proxy";
            _parser.Password = "a_password";
            Assert.IsNull(_parser.Build());
        }

        [TestMethod]
        public void SchemaEmptyProxyNullPasswordNullBuildsEmpty()
        {
            _parser.SchemaUser = string.Empty;
            _parser.ProxyUser = null;
            _parser.Password = null;
            Assert.AreEqual(string.Empty, _parser.Build());
        }

        [TestMethod]
        public void SchemaEmptyProxyNullPasswordEmptyBuildsSlash()
        {
            _parser.SchemaUser = string.Empty;
            _parser.ProxyUser = null;
            _parser.Password = string.Empty;
            Assert.AreEqual("/", _parser.Build());
        }

        [TestMethod]
        public void SchemaEmptyProxyNullPasswordFilledBuildsSlashPassword()
        {
            _parser.SchemaUser = string.Empty;
            _parser.ProxyUser = null;
            _parser.Password = "a_password";
            Assert.AreEqual("/a_password", _parser.Build());
        }

        [TestMethod]
        public void SchemaEmptyProxyEmptyPasswordNullBuildsBrackets()
        {
            _parser.SchemaUser = string.Empty;
            _parser.ProxyUser = string.Empty;
            _parser.Password = null;
            Assert.AreEqual("[]", _parser.Build());
        }

        [TestMethod]
        public void SchemaEmptyProxyEmptyPasswordEmptyBuildsBracketsAndSlash()
        {
            _parser.SchemaUser = string.Empty;
            _parser.ProxyUser = string.Empty;
            _parser.Password = string.Empty;
            Assert.AreEqual("[]/", _parser.Build());
        }

        [TestMethod]
        public void SchemaEmptyProxyEmptyPasswordFilledBuildsBracketsAndPassword()
        {
            _parser.SchemaUser = string.Empty;
            _parser.ProxyUser = string.Empty;
            _parser.Password = "a_password";
            Assert.AreEqual("[]/a_password", _parser.Build());
        }

        [TestMethod]
        public void SchemaEmptyProxyFilledPasswordNullBuildsProxyAndBrackets()
        {
            _parser.SchemaUser = string.Empty;
            _parser.ProxyUser = "a_proxy";
            _parser.Password = null;
            Assert.AreEqual("a_proxy[]", _parser.Build());
        }

        [TestMethod]
        public void SchemaEmptyProxyFilledPasswordEmptyBuildsProxyBracketsAndSlash()
        {
            _parser.SchemaUser = string.Empty;
            _parser.ProxyUser = "a_proxy";
            _parser.Password = string.Empty;
            Assert.AreEqual("a_proxy[]/", _parser.Build());
        }

        [TestMethod]
        public void SchemaEmptyProxyFilledPasswordFilledBuildsProxyBracketsAndPassword()
        {
            _parser.SchemaUser = string.Empty;
            _parser.ProxyUser = "a_proxy";
            _parser.Password = "a_password";
            Assert.AreEqual("a_proxy[]/a_password", _parser.Build());
        }

        [TestMethod]
        public void SchemaFilledProxyNullPasswordNullBuildsUser()
        {
            _parser.SchemaUser = "a_user";
            _parser.ProxyUser = null;
            _parser.Password = null;
            Assert.AreEqual("a_user", _parser.Build());
        }

        [TestMethod]
        public void SchemaFilledProxyNullPasswordEmptyBuildsUserAndSlash()
        {
            _parser.SchemaUser = "a_user";
            _parser.ProxyUser = null;
            _parser.Password = string.Empty;
            Assert.AreEqual("a_user/", _parser.Build());
        }

        [TestMethod]
        public void SchemaFilledProxyNullPasswordFilledBuildsUserAndPassword()
        {
            _parser.SchemaUser = "a_user";
            _parser.ProxyUser = null;
            _parser.Password = "a_password";
            Assert.AreEqual("a_user/a_password", _parser.Build());
        }

        [TestMethod]
        public void SchemaFilledProxyEmptyPasswordNullBuildsBracketedUser()
        {
            _parser.SchemaUser = "a_user";
            _parser.ProxyUser = string.Empty;
            _parser.Password = null;
            Assert.AreEqual("[a_user]", _parser.Build());
        }

        [TestMethod]
        public void SchemaFilledProxyEmptyPasswordEmptyBuildsBracketedUserAndSlash()
        {
            _parser.SchemaUser = "a_user";
            _parser.ProxyUser = string.Empty;
            _parser.Password = string.Empty;
            Assert.AreEqual("[a_user]/", _parser.Build());
        }

        [TestMethod]
        public void SchemaFilledProxyEmptyPasswordFilledBuildsBracketedUserAndPassword()
        {
            _parser.SchemaUser = "a_user";
            _parser.ProxyUser = string.Empty;
            _parser.Password = "a_password";
            Assert.AreEqual("[a_user]/a_password", _parser.Build());
        }

        [TestMethod]
        public void SchemaFilledProxyFilledPasswordNullBuildsProxiedUser()
        {
            _parser.SchemaUser = "a_user";
            _parser.ProxyUser = "a_proxy";
            _parser.Password = null;
            Assert.AreEqual("a_proxy[a_user]", _parser.Build());
        }

        [TestMethod]
        public void SchemaFilledProxyFilledPasswordEmptyBuildsProxiedUserAndSlash()
        {
            _parser.SchemaUser = "a_user";
            _parser.ProxyUser = "a_proxy";
            _parser.Password = string.Empty;
            Assert.AreEqual("a_proxy[a_user]/", _parser.Build());
        }

        [TestMethod]
        public void SchemaFilledProxyFilledPasswordFilledBuildsOK()
        {
            _parser.SchemaUser = "a_user";
            _parser.ProxyUser = "a_proxy";
            _parser.Password = "a_password";
            Assert.AreEqual("a_proxy[a_user]/a_password", _parser.Build());
        }
    }
}