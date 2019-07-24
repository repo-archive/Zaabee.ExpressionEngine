namespace Zaabee.ExpressionEngine.Operations
{
    internal sealed class CommaOperation : ControlOperation
    {
        public override string Code => ",";

        public CommaOperation() { }

        public override void Process()
        {
            var operatorStack = BuildingContext.Current.OperationStack;

            while (operatorStack.Count > 0 && !(operatorStack.Peek() is CloseOperation))
            {
                BuildingContext.PushExpressions(operatorStack.Pop().Apply(","));
            };

            if (operatorStack.Count == 0)
                throw new InvalidExpressionStringException("Comma operation(,) is only available in closed operation.");

            var expressionStack = BuildingContext.Current.ExpressionStack;

            if(expressionStack.Count > 0 && expressionStack.Peek().BackOperation == null)
            {
                expressionStack.Peek().BackOperation = this;
            }
            else
            {
                throw new InvalidExpressionStringException("Redundant ','.");
            }
        }

        private static Operation Build()
        {
            return OperationBuilder.FixedBuild<CommaOperation>(",");
        }
    }
}
