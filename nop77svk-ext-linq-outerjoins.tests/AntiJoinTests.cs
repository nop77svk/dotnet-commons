namespace NoP77svk.Linq.Ext.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NoP77svk.Linq.Ext;
    using System;
    using System.Collections.Generic;
    using System.Text;

    [TestClass]
    public class AntiJoinTests
    {
        private static IEnumerable<ValueTuple<string, int, char>> collection1 = new ValueTuple<string, int, char>[] {
            ("a", 1, 'a'),
            ("a", 2, 'b'),
            ("a", 3, 'c'),
            ("b", 1, 'd'),
            ("b", 2, 'e'),
            ("b", 3, 'f'),
            ("c", 1, 'g'),
            ("c", 2, 'h'),
            ("c", 3, 'i')
        };

        private static IEnumerable<ValueTuple<string, int, int>> collection2 = new ValueTuple<string, int, int>[] {
            ("a", 1, -1),
            ("a", 3, -2),
            ("a", 5, -3),
            ("c", 1, -4),
            ("c", 3, -5),
            ("c", 5, -6),
            ("e", 1, -7),
            ("e", 3, -8),
            ("e", 5, -9)
        };

        [TestMethod()]
        public void AntiJoinTest1()
        {
            var testResult = collection1
                .AntiJoin(
                    antiJoinedTable: collection2,
                    outerKeySelector: row => row.Item1,
                    antiJoinKeySelector: row => row.Item1
                );
            var expectedResult = new ValueTuple<string, int, char>[]
            {
                ("a", 2, 'b'),
                ("b", 2, 'e'),
                ("c", 2, 'h')
            };

            Assert.AreEqual(expectedResult, testResult);
        }

        [TestMethod()]
        public void AntiJoinTest2()
        {
            var testResult = collection2
                .AntiJoin(
                    antiJoinedTable: collection1,
                    outerKeySelector: row => row.Item1,
                    antiJoinKeySelector: row => row.Item1
                );
            var expectedResult = new ValueTuple<string, int, int>[]
            {
                ("e", 1, -7),
                ("e", 3, -8),
                ("e", 5, -9)
            };

            Assert.AreEqual(expectedResult, testResult);
        }
    }
}