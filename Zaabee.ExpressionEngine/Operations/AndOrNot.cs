using System.Collections.Generic;
using System.Linq.Expressions;

namespace Zaabee.ExpressionEngine.Operations
{
    internal sealed class AndAlsoOperation : BinaryOperation
    {
        public override string Code => "and";
        public override int FrontPrecedence => 40;
        public override int BackPrecedence => 40;

        public AndAlsoOperation() : base(Expression.AndAlso)
        {
        }

        private static Operation Build()
        {
            return OperationBuilder.LetterBuild<AndAlsoOperation>("and");
        }

        public override IEnumerable<Expression> Apply(string triggerStartOperation)
        {
            var expressionStack = BuildingContext.Current.ExpressionStack;
            var right = expressionStack.PopByFront(this);
            var left = expressionStack.PopByBack(this);

            if (right.Type != BooleanType || left.Type != BooleanType)
            {
                throw new InvalidExpressionStringException("And operation is only available for boolean operand.");
            }

            return new[] {Operation(left, right)};
        }
    }

    internal sealed class OrElseOperation : BinaryOperation
    {
        public override string Code => "or";
        public override int FrontPrecedence => 40;
        public override int BackPrecedence => 40;

        public OrElseOperation() : base(Expression.OrElse)
        {
        }

        private static Operation Build()
        {
            return OperationBuilder.LetterBuild<OrElseOperation>("or");
        }

        public override IEnumerable<Expression> Apply(string triggerStartOperation)
        {
            var expressionStack = BuildingContext.Current.ExpressionStack;

            var right = expressionStack.PopByFront(this);
            var left = expressionStack.PopByBack(this);

            if (right.Type != BooleanType || left.Type != BooleanType)
            {
                throw new InvalidExpressionStringException("Or operation is only available for boolean operand.");
            }

            return new[] {Operation(left, right)};
        }
    }

    internal sealed class NotOperation : BackUnaryOperation
    {
        public override string Code => "not";

        public override int FrontPrecedence => 42;

        public override int BackPrecedence => 42;

        public NotOperation() : base(Expression.Not)
        {
        }

        public override IEnumerable<Expression> Apply(string triggerStartOperation)
        {
            var expressionStack = BuildingContext.Current.ExpressionStack;

            var operand = expressionStack.PopByFront(this);

            if (operand.Type != BooleanType)
                throw new InvalidExpressionStringException("Not operation is only available for boolean type.");

            return new[] {Operation(operand)};
        }

        private static Operation Build()
        {
            return OperationBuilder.LetterBuild<NotOperation>("not");
        }
    }
}