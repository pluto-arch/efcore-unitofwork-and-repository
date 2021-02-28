using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PlutoData.Specifications.EfCore
{
    internal class ParameterReplacerVisitor : ExpressionVisitor
    {
        private readonly Expression newExpression;
        private readonly ParameterExpression oldParameter;

        private ParameterReplacerVisitor(ParameterExpression oldParameter, Expression newExpression)
        {
            this.oldParameter = oldParameter;
            this.newExpression = newExpression;
        }

        internal static Expression Replace(Expression expression, ParameterExpression oldParameter, Expression newExpression)
        {
            return new ParameterReplacerVisitor(oldParameter, newExpression).Visit(expression);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            if (p == this.oldParameter)
            {
                return this.newExpression;
            }
            else
            {
                return p;
            }
        }
    }
}
