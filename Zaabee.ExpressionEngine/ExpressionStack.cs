using System.Collections.Generic;
using System.Linq.Expressions;

namespace Zaabee.ExpressionEngine
{
    internal sealed class ExpressionStack
    {
        Stack<ExpressionElement> stack = new Stack<ExpressionElement>();

        public void Clear()
        {
            stack.Clear();
        }

        public int Count => stack.Count;

        public ExpressionElement Peek()
        {
            return stack.Peek();
        }

        public ExpressionElement Pop()
        {
            return stack.Pop();
        }


        public void Push(Operation frontOperation, Expression expression, Operation backOperation = null)
        {
            var element = new ExpressionElement
            {
                FrontOperation = frontOperation,
                Expression = expression,
                BackOperation = backOperation
            };

            stack.Push(element);
        }

        public Expression PopByFront(Operation frontOperation)
        {
            if (stack.Count > 0)
            {
                var expression = stack.Peek();

                if (expression.FrontOperation == frontOperation)
                {
                    return stack.Pop().Expression;
                }
            }

            throw new InvalidExpressionStringException($"{frontOperation.GetType().Name} has no enough back operands.");
        }

        public Expression PopByBack(Operation backOperation)
        {
            if (stack.Count > 0)
            {
                var expression = stack.Peek();

                if (expression.BackOperation == backOperation)
                {
                    return stack.Pop().Expression;
                }

                if (expression.FrontOperation == backOperation)
                    throw new InvalidExpressionStringException(
                        $"{backOperation.GetType().Name} has too back operands.");
            }

            throw new InvalidExpressionStringException($"{backOperation.GetType().Name} has no enough front operands.");
        }
    }

    internal sealed class ExpressionElement
    {
        public Operation FrontOperation { get; set; }
        public Operation BackOperation { get; set; }
        public Expression Expression { get; set; }
    }
}