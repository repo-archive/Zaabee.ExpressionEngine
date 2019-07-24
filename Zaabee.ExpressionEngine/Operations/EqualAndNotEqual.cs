using System.Collections.Generic;
using System.Linq.Expressions;

namespace Zaabee.ExpressionEngine.Operations
{
    internal sealed class EqualOperation : BinaryOperation
    {
        public override string Code => "==";
        public override int FrontPrecedence => 46;
        public override int BackPrecedence => 46;

        public EqualOperation() : base(Expression.Equal)
        {
        }

        private static Operation Build() => OperationBuilder.SymbolBuild<EqualOperation>("==");

        public override IEnumerable<Expression> Apply(string triggerStartOperation)
        {
            var expressionStack = BuildingContext.Current.ExpressionStack;

            var right = expressionStack.PopByFront(this);
            var left = expressionStack.PopByBack(this);

            if (right.Type != left.Type)
                throw new InvalidExpressionStringException("Operands of equal operation should be the same type.");

            return new[] {Operation(left, right)};
        }
    }

    internal sealed class NotEqualOperation : BinaryOperation
    {
        public override string Code => "!=";
        public override int FrontPrecedence => 46;
        public override int BackPrecedence => 46;

        public NotEqualOperation() : base(Expression.NotEqual)
        {
        }

        private static Operation Build() => OperationBuilder.SymbolBuild<NotEqualOperation>("!=");

        public override IEnumerable<Expression> Apply(string triggerStartOperation)
        {
            var expressionStack = BuildingContext.Current.ExpressionStack;

            var right = expressionStack.PopByFront(this);
            var left = expressionStack.PopByBack(this);

            if (right.Type != left.Type)
                throw new InvalidExpressionStringException("Operands of not equal operation should be the same type.");

            return new[] {Operation(left, right)};
        }
    }
}