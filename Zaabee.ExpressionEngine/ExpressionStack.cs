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

        public int Count
        {
            get
            {
                return stack.Count;
            }
        }

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
            ExpressionElement element = new ExpressionElement
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
            throw new InvalidExpressionStringException(string.Format("{0} has no enough back operands.", frontOperation.GetType().Name));
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

                if(expression.FrontOperation == backOperation)
                    throw new InvalidExpressionStringException(string.Format("{0} has too back operands.", backOperation.GetType().Name));
            }
            throw new InvalidExpressionStringException(string.Format("{0} has no enough front operands.", backOperation.GetType().Name));
        }
    }

    internal sealed class ExpressionElement
    {
        public Operation FrontOperation { get; set; }
        public Operation BackOperation { get; set; }
        public Expression Expression { get; set; }
    }
}
