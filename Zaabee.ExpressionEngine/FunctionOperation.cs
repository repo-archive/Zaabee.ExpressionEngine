using Zaabee.ExpressionEngine.Operations;

namespace Zaabee.ExpressionEngine
{
    internal abstract class FunctionOperation : CloseOperation
    {
        public override void Push()
        {
            var operatorStack = BuildingContext.Current.OperationStack;
            operatorStack.Push(new ParenthesesOperation());
            operatorStack.Push(this);
        }
    }
}