using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Zaabee.ExpressionEngine
{
    internal abstract class BinaryOperation : Operation
    {
        protected readonly Func<Expression, Expression, Expression> Operation;

        protected BinaryOperation(Func<Expression, Expression, Expression> operation)
        {
            this.Operation = operation;
        }

        public override IEnumerable<Expression> Apply(string triggerStartOperation)
        {
            var expressionStack = BuildingContext.Current.ExpressionStack;

            var right = expressionStack.PopByFront(this);
            var left = expressionStack.PopByBack(this);

            if (right.Type != NumericType || left.Type != NumericType)
            {
                throw new InvalidExpressionStringException(
                    $"{this.GetType().Name} is only available for numeric operand.");
            }

            return new[] {Operation(left, right)};
        }
    }
}