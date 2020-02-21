using System;
using Xunit;
using Lib;

namespace UnitTest
{
    public class UnitTestMain
    {
        [Fact]
        public void TestAndSpec()
        {
            var twoNumbers = new TwoNumbers(10, 20);
            var checkUpper = new UpperLimitSpec();
            var checkLower = new LowerLimitSpec();
            var checkBoth = checkUpper.And(checkLower);
            Assert.True(checkBoth.IsSatisfiedBy(twoNumbers));
        }

        [Fact]
        public void TestOrSpec()
        {
            var twoNumbers = new TwoNumbers(1000, 20);
            var checkUpper = new UpperLimitSpec();
            var checkLower = new LowerLimitSpec();
            var checkBoth = checkUpper.Or(checkLower);
            Assert.True(checkBoth.IsSatisfiedBy(twoNumbers));
        }

        [Fact] 
        public void TestNotSpec()
        {
            var twoNumbers = new TwoNumbers(1000, 20);
            var checkUpper = new UpperLimitSpec();
            var checkNot = checkUpper.Not();
            Assert.True(checkNot.IsSatisfiedBy(twoNumbers));
        }

    }



}
