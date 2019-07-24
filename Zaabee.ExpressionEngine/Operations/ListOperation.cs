using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Zaabee.ExpressionEngine.Operations
{
    internal sealed class ListOperation : CloseOperation
    {
        public override string Code
        {
            get { return "["; }
        }

        public override string CloseCode
        {
            get
            {
                return "]";
            }
        }

        public ListOperation() { }

        public override IEnumerable<Expression> Apply(string triggerStartOperation)
        {
            if (triggerStartOperation != CloseCode)
                throw new InvalidExpressionStringException("Lost ']' for '['.");

            var expressionStack = BuildingContext.Current.ExpressionStack;

            if (expressionStack.Count > 0 && expressionStack.Peek().BackOperation != null)
            {
                throw new InvalidExpressionStringException("Invalid list [].");
            }

            Expression first = expressionStack.PopByFront(this);

            List<Expression> list = new List<Expression>() { first };

            while (expressionStack.Count > 0)
            {
                var opPair = expressionStack.Peek();

                if (opPair.FrontOperation == this)
                {
                    list.Add(expressionStack.PopByFront(this));
                }
                else
                {
                    break;
                }
            }

            foreach (var expression in list)
            {
                if (expression.Type != Operation.StringType || expression.NodeType != ExpressionType.Constant)
                {
                    throw new InvalidExpressionStringException("List option is only available for string type.");
                }
            }

            Type liststringType = typeof(List<string>);

            return new Expression[] { Expression.ListInit(Expression.New(liststringType), list) };
        }

        private static Operation Build()
        {
            var reader = BuildingContext.Current.ExpressionReader;

            if (reader.Peek() == '[')
            {
                reader.Read();
                return new ListOperation();
            }

            if (reader.Peek() == ']')
            {
                reader.Read();
                return new ListClosedOperation();
            }
            return null;
        }
    }
    internal sealed class ListClosedOperation : ControlOperation
    {
        public override string Code { get { return "]"; } }

        public ListClosedOperation() { }

        public override void Process()
        {
            var operatorStack = BuildingContext.Current.OperationStack;

            while (operatorStack.Count > 0 && !(operatorStack.Peek() is ListOperation))
            {
                BuildingContext.PushExpressions(operatorStack.Pop().Apply("]"));
            };

            if (operatorStack.Count == 0)
                throw new InvalidExpressionStringException("Redundant ']'.");
            //apply [ operation
            BuildingContext.PushExpressions(operatorStack.Pop().Apply("]"));
        }
    }
}
