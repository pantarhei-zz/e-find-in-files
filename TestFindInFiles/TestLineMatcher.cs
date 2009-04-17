using System;
using System.Collections.Generic;
using FindInFiles;
using Xunit;
using Xunit.Extensions;

namespace TestFindInFiles
{
    public class TestLineMatcher
    {
        [Fact]
        public void TestFind()
        {
            FindLineOptions o = new FindLineOptions("Woot", true, false);
            var f = new LineMatcher(o);
            var s = new List<Match>(f.Filter(new string[] {@"testfiles\files1\TextFile1.txt",@"testfiles\files1\TextFile2.txt",@"testfiles\files1\TextFile3.txt"}));
            Assert.Equal(1, s.Count);
        }
    }
}
