using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Zaabee.ExpressionEngine.Operations
{
    internal sealed class StartWithInOperation : BinaryOperation
    {
        const string code = "startwithin";

        public override string Code => code;
        public override int FrontPrecedence => 46;
        public override int BackPrecedence => 46;

        public StartWithInOperation() : base(null)
        {
        }

        public override IEnumerable<Expression> Apply(string triggerStartOperation)
        {
            var expressionStack = BuildingContext.Current.ExpressionStack;

            var right = expressionStack.PopByFront(this);

            var left = expressionStack.PopByBack(this);

            if (right.Type != typeof(List<string>))
                throw new InvalidExpressionStringException(
                    "The operand followed with startWithin operation should be a string list.");

            if (left.Type != StringType)
                throw new InvalidExpressionStringException("startWithin option is only available for string type.");

            var param = Expression.Parameter(StringType, "element");

            var startWith = StringType.GetMethod("StartsWith", new Type[] {typeof(string)});

            var predicateExpressionBody = Expression.Call(left, startWith, param);

            var predicateExpression = Expression.Lambda<Predicate<string>>(predicateExpressionBody, param);

            var listStringType = typeof(List<string>);

            var exists = listStringType.GetMethod("Exists");

            return new Expression[] {Expression.Call(right, exists, predicateExpression)};
        }

        private static Operation Build()
        {
            return OperationBuilder.LetterBuild<StartWithInOperation>(code);
        }
    }
}