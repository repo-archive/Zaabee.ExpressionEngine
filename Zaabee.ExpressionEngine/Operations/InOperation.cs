using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Zaabee.ExpressionEngine.Operations
{
    internal sealed class InOperation : BinaryOperation
    {
        const string code = "in";
        public override string Code
        {
            get { return code; }
        }
        public override int FrontPrecedence { get { return 46; } }
        public override int BackPrecedence { get { return 46; } }

        public InOperation() : base(null) { }

        public override IEnumerable<Expression> Apply(string triggerStartOperation)
        {
            var expressionStack = BuildingContext.Current.ExpressionStack;

            Expression right = expressionStack.PopByFront(this);

            Expression left = expressionStack.PopByBack(this);

            if(right.Type != typeof(List<string>))
            {
                throw new InvalidExpressionStringException("The operand followed with in operaion should be a string list.");
            }

            if (left.Type != Operation.StringType)
            {
                throw new InvalidExpressionStringException("In option is only available for string type.");
            }

            Type liststringType = typeof(List<string>);

            MethodInfo contains = liststringType.GetMethod("Contains");

            return new Expression[] { Expression.Call(right, contains, left) };
        }

        private static Operation Build()
        {
            return OperationBuilder.LetterBuild<InOperation>(code);
        }
    }
}
