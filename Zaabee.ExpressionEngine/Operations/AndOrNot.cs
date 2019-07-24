using System.Collections.Generic;
using System.Linq.Expressions;

namespace Zaabee.ExpressionEngine.Operations
{
    internal sealed class AndAlsoOperation : BinaryOperation
    {
        public override string Code { get { return "and"; } }
        public override int FrontPrecedence { get { return 40; } }
        public override int BackPrecedence { get { return 40; } }

        public AndAlsoOperation() : base(Expression.AndAlso) { }

        private static Operation Build()
        {
            return OperationBuilder.LetterBuild<AndAlsoOperation>("and");
        }

        public override IEnumerable<Expression> Apply(string triggerStartOperation)
        {
            var expressionStack = BuildingContext.Current.ExpressionStack;
            Expression right = expressionStack.PopByFront(this);
            Expression left = expressionStack.PopByBack(this);

            if (right.Type != Operation.BooleanType || left.Type != Operation.BooleanType)
            {
                throw new InvalidExpressionStringException("And operation is only available for boolean operand.");
            }

            return new Expression[] { operation(left, right) };
        }
    }
    internal sealed class OrElseOperation : BinaryOperation
    {
        public override string Code { get { return "or"; } }
        public override int FrontPrecedence { get { return 40; } }
        public override int BackPrecedence { get { return 40; } }

        public OrElseOperation() : base(Expression.OrElse) { }

        private static Operation Build()
        {
            return OperationBuilder.LetterBuild<OrElseOperation>("or");
        }

        public override IEnumerable<Expression> Apply(string triggerStartOperation)
        {
            var expressionStack = BuildingContext.Current.ExpressionStack;

            Expression right = expressionStack.PopByFront(this);
            Expression left = expressionStack.PopByBack(this);

            if (right.Type != Operation.BooleanType || left.Type != Operation.BooleanType)
            {
                throw new InvalidExpressionStringException("Or operation is only available for boolean operand.");
            }

            return new Expression[] { operation(left, right) };
        }
    }

    internal sealed class NotOperation : BackUnaryOperation
    {
        public override string Code
        {
            get { return "not"; }
        }

        public override int FrontPrecedence
        {
            get { return 42; }
        }

        public override int BackPrecedence
        {
            get { return 42; }
        }

        public NotOperation() : base(Expression.Not) { }

        public override IEnumerable<Expression> Apply(string triggerStartOperation)
        {
            var expressionStack = BuildingContext.Current.ExpressionStack;

            Expression operand = expressionStack.PopByFront(this);

            if (operand.Type != Operation.BooleanType)
                throw new InvalidExpressionStringException("Not operation is only available for boolean type.");

            return new Expression[] { operation(operand) };
        }

        private static Operation Build()
        {
            return OperationBuilder.LetterBuild<NotOperation>("not");
        }
    }
}
