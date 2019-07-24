using System.Collections.Generic;
using System.Linq.Expressions;

namespace Zaabee.ExpressionEngine.Operations
{
    internal sealed class IfElseThenOperation : Operation
    {
        public override string Code => "if then else";
        public override int FrontPrecedence => 100;
        public override int BackPrecedence => 0;

        public override IEnumerable<Expression> Apply(string triggerStartOperation)
        {
            var expressionStack = BuildingContext.Current.ExpressionStack;

            var third = expressionStack.PopByFront(this);
            var second = expressionStack.PopByFront(this);
            var first = expressionStack.PopByFront(this);

            if (first.Type != Operation.BooleanType)
                throw new InvalidExpressionStringException("Only logic operand can follow with 'if' operation.");

            if (expressionStack.Count > 0 && expressionStack.Peek().FrontOperation == this)
                throw new InvalidExpressionStringException("If_then_else operation have more than 3 operands.");

            return new Expression[] {Expression.Condition(first, second, third)};
        }

        private static Operation Build()
        {
            var reader = BuildingContext.Current.ExpressionReader;

            if (reader.Peek(2) == "if" && !reader.IsLetterOrDigit(2))
            {
                reader.Read(2);
                return new IfElseThenOperation();
            }

            if (reader.Peek(4) == "then" && !reader.IsLetterOrDigit(4))
            {
                reader.Read(4);
                return new ThenOperation();
            }

            if (reader.Peek(4) == "else" && !reader.IsLetterOrDigit(4))
            {
                reader.Read(4);
                return new ElseOperation();
            }

            return null;
        }
    }

    internal sealed class ThenOperation : ControlOperation
    {
        public override string Code => "then";

        public override void Process()
        {
            var operatorStack = BuildingContext.Current.OperationStack;

            while (operatorStack.Count > 0 && !(operatorStack.Peek() is IfElseThenOperation))
                BuildingContext.PushExpressions(operatorStack.Pop().Apply("then"));

            if (operatorStack.Count == 0)
                throw new InvalidExpressionStringException("Lost 'if' for 'then'.");

            var expressionStack = BuildingContext.Current.ExpressionStack;

            if (expressionStack.Count > 0 && expressionStack.Peek().BackOperation == null)
                expressionStack.Peek().BackOperation = this;
        }
    }

    internal sealed class ElseOperation : ControlOperation
    {
        public override string Code => "else";

        public override void Process()
        {
            var operatorStack = BuildingContext.Current.OperationStack;

            while (operatorStack.Count > 0 && !(operatorStack.Peek() is IfElseThenOperation))
                BuildingContext.PushExpressions(operatorStack.Pop().Apply("else"));

            if (operatorStack.Count == 0)
                throw new InvalidExpressionStringException("Lost 'if' and 'then' for 'else'.");

            var expressionStack = BuildingContext.Current.ExpressionStack;

            if (expressionStack.Count > 0 && expressionStack.Peek().BackOperation == null)
                expressionStack.Peek().BackOperation = this;
        }
    }
}