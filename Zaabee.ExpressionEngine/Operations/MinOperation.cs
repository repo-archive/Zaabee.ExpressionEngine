using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Zaabee.ExpressionEngine.Operations
{
    internal sealed class MinOperation : FunctionOperation
    { 
        const string code = "min";
        public override string Code
        {
            get { return code; }
        }

        public override string CloseCode
        {
            get { return ")"; }
        }
        public MinOperation() { }

        public override IEnumerable<Expression> Apply(string triggerStartOperation)
        {
            if (triggerStartOperation != CloseCode)
                throw new InvalidExpressionStringException("Lost ')' for min() function.");

            var expressionStack = BuildingContext.Current.ExpressionStack;

            if(expressionStack.Count > 0 && expressionStack.Peek().BackOperation != null)
            {
                throw new InvalidExpressionStringException("Invalid min() function.");
            }

            Expression second = expressionStack.PopByFront(this);
            Expression first = expressionStack.PopByFront(this);

            if (expressionStack.Count > 0 && expressionStack.Peek().FrontOperation == this)
                throw new InvalidExpressionStringException("Min operation have more than 2 operands.");

            if (first.Type != Operation.NumericType || second.Type != Operation.NumericType)
                throw new InvalidExpressionStringException("Min operation is only available for numeric type.");

            MethodInfo method = typeof(Math).GetMethod("Min", new Type[] { Operation.NumericType, Operation.NumericType });

            return new Expression[] { Expression.Call(method, first, second) };
        }

        private static Operation Build()
        {
            return OperationBuilder.FuncBuild<MinOperation>(code);
        }
    }
}
