using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Zaabee.ExpressionEngine
{
    internal abstract class BackUnaryOperation : Operation
    {
        protected readonly Func<Expression, Expression> Operation;

        protected BackUnaryOperation(Func<Expression, Expression> operation) => Operation = operation;

        public override IEnumerable<Expression> Apply(string triggerStartOperation)
        {
            var expressionStack = BuildingContext.Current.ExpressionStack;

            var operand = expressionStack.PopByFront(this);

            return new[] {Operation(operand)};
        }
    }
}