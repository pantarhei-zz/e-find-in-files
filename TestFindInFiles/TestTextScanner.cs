using System;
using System.Collections.Generic;
using FindInFiles;
using Xunit;
using Xunit.Extensions;

namespace TestFindInFiles
{
    public class TestTextScanner
    {
        [Theory]
        [InlineData(true, 1)]
        [InlineData(false, 3)]
        public void TestScan(bool matchCase, int expectedMatches)
        {
            TextScanner t = new TextScanner("find me", matchCase);
            var s = new List<IntRange>(t.Scan("abc find me DEF FIND ME ghi FiNd Me jkl"));

            Assert.Equal(expectedMatches, s.Count);
        }
    }
}
