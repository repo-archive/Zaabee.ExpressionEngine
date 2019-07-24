using System.Linq.Expressions;

namespace Zaabee.ExpressionEngine.Operations
{
    internal sealed class GreaterThanOperation : BinaryOperation
    {
        public override string Code => ">";
        public override int FrontPrecedence => 46;
        public override int BackPrecedence => 46;

        public GreaterThanOperation() : base(Expression.GreaterThan)
        {
        }

        private static Operation Build()
        {
            return OperationBuilder.SymbolBuild<GreaterThanOperation>(">");
        }
    }

    internal sealed class GreaterThanOrEqualOperation : BinaryOperation
    {
        public override string Code => ">=";
        public override int FrontPrecedence => 46;
        public override int BackPrecedence => 46;

        public GreaterThanOrEqualOperation() : base(Expression.GreaterThanOrEqual)
        {
        }

        private static Operation Build() => OperationBuilder.SymbolBuild<GreaterThanOrEqualOperation>(">=");
    }

    internal sealed class LessThanOperation : BinaryOperation
    {
        public override string Code => "<";
        public override int FrontPrecedence => 46;
        public override int BackPrecedence => 46;

        public LessThanOperation() : base(Expression.LessThan)
        {
        }

        private static Operation Build() => OperationBuilder.SymbolBuild<LessThanOperation>("<");
    }

    internal sealed class LessThanOrEqualOperation : BinaryOperation
    {
        public override string Code => "<=";
        public override int FrontPrecedence => 46;
        public override int BackPrecedence => 46;

        public LessThanOrEqualOperation() : base(Expression.LessThanOrEqual)
        {
        }

        private static Operation Build() => OperationBuilder.SymbolBuild<LessThanOrEqualOperation>("<=");
    }
}