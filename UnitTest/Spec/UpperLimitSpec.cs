using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

using Lib;

namespace UnitTest
{
    internal class UpperLimitSpec : Specification<TwoNumbers>
    {
        private const int _upperLimit = 1000;
        public override Expression<Func<TwoNumbers, bool>> ToExpression()
        {
            return twoNumbers => twoNumbers.First + twoNumbers.Second <= _upperLimit; 
        }
    }
}
