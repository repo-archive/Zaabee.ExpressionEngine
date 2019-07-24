using System.Collections.Generic;
using System.Linq.Expressions;

namespace Zaabee.ExpressionEngine.Operations
{
    internal sealed class InOperation : BinaryOperation
    {
        const string code = "in";
        public override string Code => code;
        public override int FrontPrecedence => 46;
        public override int BackPrecedence => 46;

        public InOperation() : base(null) { }

        public override IEnumerable<Expression> Apply(string triggerStartOperation)
        {
            var expressionStack = BuildingContext.Current.ExpressionStack;

            var right = expressionStack.PopByFront(this);

            var left = expressionStack.PopByBack(this);

            if(right.Type != typeof(List<string>))
            {
                throw new InvalidExpressionStringException("The operand followed with in operation should be a string list.");
            }

            if (left.Type != StringType)
            {
                throw new InvalidExpressionStringException("In option is only available for string type.");
            }

            var listStringType = typeof(List<string>);

            var contains = listStringType.GetMethod("Contains");

            return new Expression[] { Expression.Call(right, contains, left) };
        }

        private static Operation Build()
        {
            return OperationBuilder.LetterBuild<InOperation>(code);
        }
    }
}
