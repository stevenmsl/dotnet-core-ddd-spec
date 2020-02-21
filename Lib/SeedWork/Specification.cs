using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Lib
{
    #region Helper classes not exposed to the clients  
    internal sealed class IdentitySpecification<T> : Specification<T>
    {
        public override Expression<Func<T, bool>> ToExpression()
        {
            return x => true;
        }
    }
    internal sealed class AndSpecification<T> : Specification<T>
    {
        private readonly Specification<T> _left;
        private readonly Specification<T> _right;
        public AndSpecification(Specification<T> left, Specification<T> right)
        {
            _right = right;
            _left = left;
        }

        /*
         *  So the idea here is to create an expression that would invoke 
         *  the left and right expressions with the same parameters and then 
         *  apply && operator to combine these two results.
         */
        public override Expression<Func<T, bool>> ToExpression()
        {
            var left = _left.ToExpression();
            var right = _right.ToExpression();

            var invoked = Expression.Invoke(right, left.Parameters);

            /* This is what it would look like after applying the AndAlso operator 
             * 
               .Lambda #Lambda1<System.Func`2[UnitTest.TwoNumbers,System.Boolean]>(UnitTest.TwoNumbers $twoNumbers) {
               $twoNumbers.First + $twoNumbers.Second <= 1000 && .Invoke (.Lambda #Lambda2<System.Func`2[UnitTest.TwoNumbers,System.Boolean]>)($twoNumbers)
               }

               .Lambda #Lambda2<System.Func`2[UnitTest.TwoNumbers,System.Boolean]>(UnitTest.TwoNumbers $twoNumbers) {
                    $twoNumbers.First + $twoNumbers.Second >= 10
               }                           
            */

            var combined =  (Expression < Func<T, bool> >) Expression.Lambda                    
                    (Expression.AndAlso(left.Body, invoked), left.Parameters);

            return combined;
        }

            
               

    }

    internal sealed class OrSpecification<T> : Specification<T>
    {
        private readonly Specification<T> _left;
        private readonly Specification<T> _right;
        public OrSpecification(Specification<T> left, Specification<T> right)
        {
            _right = right;
            _left = left;
        }

        public override Expression<Func<T, bool>> ToExpression()
        {
            var left = _left.ToExpression();
            var right = _right.ToExpression();

            var invoked = Expression.Invoke(right, left.Parameters);

            /* This is what it would look like after applying the AndAlso operator 
             * 
            .Lambda #Lambda1<System.Func`2[UnitTest.TwoNumbers,System.Boolean]>(UnitTest.TwoNumbers $twoNumbers) {
                $twoNumbers.First + $twoNumbers.Second <= 1000 || .Invoke (.Lambda #Lambda2<System.Func`2[UnitTest.TwoNumbers,System.Boolean]>)($twoNumbers)
            }

            .Lambda #Lambda2<System.Func`2[UnitTest.TwoNumbers,System.Boolean]>(UnitTest.TwoNumbers $twoNumbers) {
                $twoNumbers.First + $twoNumbers.Second >= 10
            }                        
            */

            var combined = (Expression<Func<T, bool>>)Expression.Lambda
                    (Expression.OrElse(left.Body, invoked), left.Parameters);

            return combined;
        }
    }

    internal sealed class NotSpecification<T> : Specification<T>
    {
        private readonly Specification<T> _spec;
        
        public NotSpecification(Specification<T> spec)
        {
            _spec = spec;
        }

        public override Expression<Func<T, bool>> ToExpression()
        {
            var expr = _spec.ToExpression();
            var notExpr = Expression.Not(expr.Body);

            /*
             * 
             * .Lambda #Lambda1<System.Func`2[UnitTest.TwoNumbers,System.Boolean]>(UnitTest.TwoNumbers $twoNumbers) {
               !($twoNumbers.First + $twoNumbers.Second <= 1000)
               */
            var converted = Expression.Lambda<Func<T, bool>>(notExpr, expr.Parameters);
            return converted;


        }

        
    }



    #endregion

    // this is the only class type exposed to the client code
    // all other class types are internal
    public abstract class Specification<T>
    {
        public static readonly Specification<T> All = new IdentitySpecification<T>();
        public abstract Expression<Func<T, bool>> ToExpression();

        public bool IsSatisfiedBy(T entity)
        {
            // Compile will return you a delegate that represents 
            // the lambda expression
            var func = ToExpression().Compile();

            return func(entity);
        }

        public Specification<T> And(Specification<T> spec)
        {
            if (this == null) return spec;
            if (spec == All) return this;
            return new AndSpecification<T>(this, spec);
        }

        public Specification<T> Or(Specification<T> spec)
        {
            // no further evaluation is required
            // as the result will be always true
            if (this == All || spec == All) return All;
            return new OrSpecification<T>(this, spec);
        }

        public Specification<T> Not()
        {
            return new NotSpecification<T>(this);
        } 


    }
}
