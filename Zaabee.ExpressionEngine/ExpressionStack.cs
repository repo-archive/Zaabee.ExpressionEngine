using System.Collections.Generic;
using System.Linq.Expressions;

namespace Zaabee.ExpressionEngine
{
    internal sealed class ExpressionStack
    {
        private readonly Stack<ExpressionElement> _stack = new Stack<ExpressionElement>();
        public void Clear() => _stack.Clear();
        public int Count => _stack.Count;
        public ExpressionElement Peek() => _stack.Peek();
        public ExpressionElement Pop() => _stack.Pop();

        public void Push(Operation frontOperation, Expression expression, Operation backOperation = null)
        {
            var element = new ExpressionElement
            {
                FrontOperation = frontOperation,
                Expression = expression,
                BackOperation = backOperation
            };

            _stack.Push(element);
        }

        public Expression PopByFront(Operation frontOperation)
        {
            if (_stack.Count <= 0)
                throw new InvalidExpressionStringException(
                    $"{frontOperation.GetType().Name} has no enough back operands.");
            var expression = _stack.Peek();

            if (expression.FrontOperation == frontOperation)
                return _stack.Pop().Expression;
            throw new InvalidExpressionStringException($"{frontOperation.GetType().Name} has no enough back operands.");
        }

        public Expression PopByBack(Operation backOperation)
        {
            if (_stack.Count <= 0)
                throw new InvalidExpressionStringException(
                    $"{backOperation.GetType().Name} has no enough front operands.");
            var expression = _stack.Peek();

            if (expression.BackOperation == backOperation)
                return _stack.Pop().Expression;

            if (expression.FrontOperation == backOperation)
                throw new InvalidExpressionStringException($"{backOperation.GetType().Name} has too back operands.");
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