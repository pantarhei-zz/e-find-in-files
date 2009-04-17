using System;
using System.Collections.Generic;
using FindInFiles;
using Xunit;
using Xunit.Extensions;

namespace TestFindInFiles
{
    public class TestFileMatcher
    {
        [Fact]
        public void TestFind()
        {
            FindFileOptions o = new FindFileOptions(@"testfiles\files1", new string[] {}, new string[] {});
            var f = new FileMatcher(o);
            var r = new List<string>(f.Filter());
            Assert.Equal(3, r.Count);
        }
    }
}
