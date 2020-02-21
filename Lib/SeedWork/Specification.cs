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

        public override Expression<Func<T, bool>> ToExpression()
        {
            var left = _left.ToExpression();
            var right = _right.ToExpression();

            var invoked = Expression.Invoke(right, left.Parameters);

            return
                (Expression < Func<T, bool> >) Expression.Lambda                    
                    (Expression.AndAlso(left.Body, invoked), left.Parameters);                                
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

    }
}
