using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Zaabee.ExpressionEngine.Operations
{
    internal sealed class StartWithInOperation : BinaryOperation
    {
        const string code = "startwithin";

        public override string Code
        {
            get { return code; }
        }
        public override int FrontPrecedence { get { return 46; } }
        public override int BackPrecedence { get { return 46; } }


        public StartWithInOperation() : base(null) { }

        public override IEnumerable<Expression> Apply(string triggerStartOperation)
        {
            var expressionStack = BuildingContext.Current.ExpressionStack;

            Expression right = expressionStack.PopByFront(this);

            Expression left = expressionStack.PopByBack(this);

            if (right.Type != typeof(List<string>))
            {
                throw new InvalidExpressionStringException("The operand followed with startwithin operaion should be a string list.");
            }

            if (left.Type != Operation.StringType)
            {
                throw new InvalidExpressionStringException("Startwithin option is only available for string type.");
            }

            ParameterExpression param = Expression.Parameter(Operation.StringType, "element");

            MethodInfo startWith = Operation.StringType.GetMethod("StartsWith", new Type[] { typeof(string) });

            var predicateExpressionBody = Expression.Call(left, startWith, param);

            Expression<Predicate<string>> predicateExpression = LambdaExpression.Lambda<Predicate<string>>(predicateExpressionBody, param);
            
            Type liststringType = typeof(List<string>);

            MethodInfo exists = liststringType.GetMethod("Exists");

            return new Expression[] { Expression.Call(right, exists, predicateExpression) };
        }

        private static Operation Build()
        {
            return OperationBuilder.LetterBuild<StartWithInOperation>(code);
        }
    }
}
