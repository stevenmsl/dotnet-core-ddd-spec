using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Lib;

namespace UnitTest
{
    internal class LowerLimitSpec : Specification<TwoNumbers>
    {
        private const int _lowerLimit = 10;

        public override Expression<Func<TwoNumbers, bool>> ToExpression()
        {
            return twoNumbers => twoNumbers.First + twoNumbers.Second >= _lowerLimit;
        }
    }
}

