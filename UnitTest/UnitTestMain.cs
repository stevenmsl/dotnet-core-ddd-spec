using System;
using Xunit;
using Lib;

namespace UnitTest
{
    public class UnitTestMain
    {
        [Fact]
        public void TestSpec()
        {
            var twoNumbers = new TwoNumbers(10, 20);
            var checkUpper = new UpperLimitSpec();
            var checkLower = new LowerLimitSpec();
            var checkBoth = checkUpper.And(checkLower);
            Assert.True(checkBoth.IsSatisfiedBy(twoNumbers));
        }
    }



}
