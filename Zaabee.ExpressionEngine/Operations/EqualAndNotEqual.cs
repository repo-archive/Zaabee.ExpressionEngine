using System.Collections.Generic;
using System.Linq.Expressions;

namespace Zaabee.ExpressionEngine.Operations
{
    internal sealed class EqualOperation : BinaryOperation
    {
        public override string Code { get { return "=="; } }
        public override int FrontPrecedence { get { return 46; } }
        public override int BackPrecedence { get { return 46; } }

        public EqualOperation() : base(Expression.Equal) { }

        private static Operation Build()
        {
            return OperationBuilder.SymbolBuild<EqualOperation>("==");
        }

        public override IEnumerable<Expression> Apply(string triggerStartOperation)
        {
            var expressionStack = BuildingContext.Current.ExpressionStack;

            Expression right = expressionStack.PopByFront(this);
            Expression left = expressionStack.PopByBack(this);

            if (right.Type != left.Type)
            {
                throw new InvalidExpressionStringException("Operands of equal operation should be the same type.");
            }

            return new Expression[] { operation(left, right) };
        }
    }
    internal sealed class NotEqualOperation : BinaryOperation
    {
        public override string Code { get { return "!="; } }
        public override int FrontPrecedence { get { return 46; } }
        public override int BackPrecedence { get { return 46; } }

        public NotEqualOperation() : base(Expression.NotEqual) { }

        private static Operation Build()
        {
            return OperationBuilder.SymbolBuild<NotEqualOperation>("!=");
        }

        public override IEnumerable<Expression> Apply(string triggerStartOperation)
        {
            var expressionStack = BuildingContext.Current.ExpressionStack;

            Expression right = expressionStack.PopByFront(this);
            Expression left = expressionStack.PopByBack(this);

            if (right.Type != left.Type)
            {
                throw new InvalidExpressionStringException("Operands of not equal operation should be the same type.");
            }

            return new Expression[] { operation(left, right) };
        }
    }
}
