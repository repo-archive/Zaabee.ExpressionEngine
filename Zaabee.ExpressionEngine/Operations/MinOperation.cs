using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Zaabee.ExpressionEngine.Operations
{
    internal sealed class MinOperation : FunctionOperation
    { 
        const string code = "min";
        public override string Code => code;

        public override string CloseCode => ")";

        public override IEnumerable<Expression> Apply(string triggerStartOperation)
        {
            if (triggerStartOperation != CloseCode)
                throw new InvalidExpressionStringException("Lost ')' for min() function.");

            var expressionStack = BuildingContext.Current.ExpressionStack;

            if(expressionStack.Count > 0 && expressionStack.Peek().BackOperation != null)
                throw new InvalidExpressionStringException("Invalid min() function.");

            var second = expressionStack.PopByFront(this);
            var first = expressionStack.PopByFront(this);

            if (expressionStack.Count > 0 && expressionStack.Peek().FrontOperation == this)
                throw new InvalidExpressionStringException("Min operation have more than 2 operands.");

            if (first.Type != Operation.NumericType || second.Type != Operation.NumericType)
                throw new InvalidExpressionStringException("Min operation is only available for numeric type.");

            var method = typeof(Math).GetMethod("Min", new Type[] { Operation.NumericType, Operation.NumericType });

            return new Expression[] { Expression.Call(method, first, second) };
        }

        private static Operation Build()
        {
            return OperationBuilder.FuncBuild<MinOperation>(code);
        }
    }
}
