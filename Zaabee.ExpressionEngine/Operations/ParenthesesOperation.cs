using System.Collections.Generic;
using System.Linq.Expressions;

namespace Zaabee.ExpressionEngine.Operations
{
    internal sealed class ParenthesesOperation : CloseOperation
    {
        public override string Code => "(";

        public override string CloseCode => ")";

        public ParenthesesOperation()
        {
        }

        public override IEnumerable<Expression> Apply(string triggerStartOperation)
        {
            if (triggerStartOperation != CloseCode)
                throw new InvalidExpressionStringException("Lost ')' for '('.");

            var expressionStack = BuildingContext.Current.ExpressionStack;

            if (expressionStack.Count > 0 && expressionStack.Peek().BackOperation != null)
            {
                throw new InvalidExpressionStringException("Invalid () body.");
            }

            var result = expressionStack.PopByFront(this);

            if (expressionStack.Count > 0)
            {
                var opPair = expressionStack.Peek();

                if (opPair.FrontOperation == this)
                {
                    throw new InvalidExpressionStringException("Parentheses operation has more than one operand.");
                }
            }

            return new[] {result};
        }

        private static Operation Build()
        {
            var reader = BuildingContext.Current.ExpressionReader;

            if (reader.Peek() == '(')
            {
                reader.Read();
                return new ParenthesesOperation();
            }

            if (reader.Peek() == ')')
            {
                reader.Read();
                return new ParenthesesClosedOperation();
            }

            return null;
        }
    }

    internal sealed class ParenthesesClosedOperation : ControlOperation
    {
        public override string Code => ")";

        public ParenthesesClosedOperation()
        {
        }

        public override void Process()
        {
            var operatorStack = BuildingContext.Current.OperationStack;

            while (operatorStack.Count > 0 && !(operatorStack.Peek() is ParenthesesOperation))
            {
                BuildingContext.PushExpressions(operatorStack.Pop().Apply(")"));
            }

            if (operatorStack.Count == 0)
                throw new InvalidExpressionStringException("Redundant ')'.");
            //apply ( operation
            BuildingContext.PushExpressions(operatorStack.Pop().Apply(")"));
        }
    }
}