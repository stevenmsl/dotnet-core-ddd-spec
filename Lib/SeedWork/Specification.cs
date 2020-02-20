using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Lib
{
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
            throw new NotImplementedException();
        }

    } 

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
            return ToExpression().Compile()(entity);
        }

        public Specification<T> And(Specification<T> spec)
        {
            if (this == null) return spec;
            if (spec == All) return this;
            throw new NotImplementedException();
        }

    }
}
