using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Zaabee.ExpressionEngine.Operations;

namespace Zaabee.ExpressionEngine
{
    internal sealed class BuildingContext
    {
        [ThreadStatic] private static Stack<WorkSpace> _workSpaceStack;

        public static void BeginBuilding(string expression)
        {
            if (_workSpaceStack == null)
                _workSpaceStack = new Stack<WorkSpace>();

            _workSpaceStack.Push(new WorkSpace(expression, new ExpressionStack(),
                new ExpressionStringReader(expression), new Stack<Operation>()));
        }

        public static void EndBuilding()
        {
            if (_workSpaceStack.Count > 0)
                _workSpaceStack.Pop();
        }

        public static List<string> ExpressionList
        {
            get { return _workSpaceStack.Select(wss => wss.Expression).ToList(); }
        }

        public static WorkSpace Current => _workSpaceStack.Peek();

        public static void PushExpressions(IEnumerable<Expression> expressions)
        {
            var workspace = Current;

            if (workspace.ExpressionStack.Count > 0)
            {
                var expressionElement = workspace.ExpressionStack.Peek();

                if (expressionElement.BackOperation == null)
                    throw new InvalidExpressionStringException("Broken expression.");
            }

            var op = workspace.OperationStack.Count > 0 ? workspace.OperationStack.Peek() : null;

            foreach (var expression in expressions)
            {
                /*** HACK ***/
                //NOTE: if there are more than one expressions, 
                //we should asume that each of expression has a comma followed with it.                
                workspace.ExpressionStack.Push(op, expression, new CommaOperation());
            }

            if (expressions.Any())
            {
                //we should handle the last redundant back operation.
                workspace.ExpressionStack.Peek().BackOperation = null;
            }
        }

        public static void PushExpression(Expression expression)
        {
            var workspace = Current;

            if (workspace.ExpressionStack.Count > 0)
            {
                var expressionElement = workspace.ExpressionStack.Peek();

                if (expressionElement.BackOperation == null)
                    throw new InvalidExpressionStringException("Broken expression.");
            }

            var op = workspace.OperationStack.Count > 0 ? workspace.OperationStack.Peek() : null;

            workspace.ExpressionStack.Push(op, expression);
        }

        public static void PushOperation(Operation operation)
        {
            var workspace = BuildingContext.Current;

            workspace.OperationStack.Push(operation);

            if (workspace.ExpressionStack.Count <= 0) return;
            var expressionElement = workspace.ExpressionStack.Peek();

            if (expressionElement.BackOperation == null)
                expressionElement.BackOperation = operation;
        }
    }

    internal sealed class WorkSpace
    {
        public string Expression { get; private set; }
        public ExpressionStack ExpressionStack { get; private set; }
        public ExpressionStringReader ExpressionReader { get; private set; }
        public Stack<Operation> OperationStack { get; private set; }

        public WorkSpace(string expression, ExpressionStack expressionStack, ExpressionStringReader expressionReader,
            Stack<Operation> operationStack)
        {
            Expression = expression;
            ExpressionStack = expressionStack;
            ExpressionReader = expressionReader;
            OperationStack = operationStack;
        }
    }
}