using System.Collections.Generic;
using System.Linq.Expressions;

namespace Zaabee.ExpressionEngine.Operations
{
    internal sealed class AddOperation : BinaryOperation
    {
        public override string Code => "+";

        public override int FrontPrecedence => 50;
        public override int BackPrecedence => 50;

        public AddOperation() : base(Expression.Add)
        {
        }
    }

    internal sealed class SubOperation : BinaryOperation
    {
        public override string Code => "-";
        public override int FrontPrecedence => 50;
        public override int BackPrecedence => 50;

        public SubOperation() : base(Expression.Subtract)
        {
        }

        private static Operation Build()
        {
            Operation result = OperationBuilder.SymbolBuild<SubOperation>("-");

            var expressionStack = BuildingContext.Current.ExpressionStack;

            if (result != null && (expressionStack.Count == 0 || expressionStack.Peek().BackOperation != null))
                result = new NegativeOperation();
            return result;
        }
    }

    internal sealed class NegativeOperation : BackUnaryOperation
    {
        public override string Code => "-";
        public override int FrontPrecedence => 54;
        public override int BackPrecedence => 54;

        public NegativeOperation() : base(Expression.Negate)
        {
        }

        public override IEnumerable<Expression> Apply(string triggerStartOperation)
        {
            var expressionStack = BuildingContext.Current.ExpressionStack;

            var operand = expressionStack.PopByFront(this);

            if (operand.Type != NumericType)
                throw new InvalidExpressionStringException("Negative operation is only available for numeric type.");

            return new[] {Operation(operand)};
        }
    }

    internal sealed class MultiplyOperation : BinaryOperation
    {
        public override string Code => "*";
        public override int FrontPrecedence => 52;
        public override int BackPrecedence => 52;

        public MultiplyOperation() : base(Expression.Multiply)
        {
        }

        private static Operation Build() => OperationBuilder.SymbolBuild<MultiplyOperation>("*");
    }

    internal sealed class DivideOperation : BinaryOperation
    {
        public override string Code => "/";
        public override int FrontPrecedence => 52;
        public override int BackPrecedence => 52;

        public DivideOperation() : base(Expression.Divide)
        {
        }

        private static Operation Build()
        {
            return OperationBuilder.SymbolBuild<DivideOperation>("/");
        }
    }
}