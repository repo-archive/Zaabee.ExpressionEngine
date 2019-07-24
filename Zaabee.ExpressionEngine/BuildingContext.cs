using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Zaabee.ExpressionEngine.Operations;

namespace Zaabee.ExpressionEngine
{
    internal sealed class BuildingContext
    {
        [ThreadStatic]
        private static Stack<WorkSpace> WorkSacpeStack;

        public static void BeginBuilding(string expression)
        {
            if (WorkSacpeStack == null)
                WorkSacpeStack = new Stack<WorkSpace>();

            WorkSacpeStack.Push(new WorkSpace(expression, new ExpressionStack(), new ExpressionStringReader(expression), new Stack<Operation>()));
        }
        public static void EndBuilding()
        {
            if (WorkSacpeStack.Count > 0)
                WorkSacpeStack.Pop();
        }
        public static List<string> ExpressionList
        {
            get
            {
                return WorkSacpeStack.Select(wss => wss.Expression).ToList();
            }
        }
        public static WorkSpace Current
        {
            get
            {
                return WorkSacpeStack.Peek();
            }
        }
        public static void PushExpressions(IEnumerable<Expression> expressions)
        {
            var workspace = BuildingContext.Current;

            if (workspace.ExpressionStack.Count > 0)
            {
                var expressionElement = workspace.ExpressionStack.Peek();

                if (expressionElement.BackOperation == null)
                    throw new InvalidExpressionStringException("Broken expression.");
            }

            Operation op = workspace.OperationStack.Count > 0 ? workspace.OperationStack.Peek() : null;
           
            foreach (var expression in expressions)
            {
                /*** HACK ***/
                //NOTE: if there are more than one expressions, 
                //we should asume that each of expression has a comma followed with it.                
                workspace.ExpressionStack.Push(op, expression, new CommaOperation());
            }

            if(expressions.Count() > 0)
            {
                //we should handle the last redundant back operation.
                workspace.ExpressionStack.Peek().BackOperation = null;
            }
        }
        public static void PushExpression(Expression expression)
        {
            var workspace = BuildingContext.Current;

            if (workspace.ExpressionStack.Count > 0)
            {
                var expressionElement = workspace.ExpressionStack.Peek();

                if (expressionElement.BackOperation == null)
                    throw new InvalidExpressionStringException("Broken expression.");
            }

            Operation op = workspace.OperationStack.Count > 0 ? workspace.OperationStack.Peek() : null;

            workspace.ExpressionStack.Push(op, expression);
        }
        public static void PushOperation(Operation operation)
        {
            var workspace = BuildingContext.Current;

            workspace.OperationStack.Push(operation);

            if (workspace.ExpressionStack.Count > 0)
            {
                var expressionElement = workspace.ExpressionStack.Peek();

                if(expressionElement.BackOperation == null)
                {
                    expressionElement.BackOperation = operation;
                }
            }
        }
    }
    internal sealed class WorkSpace
    {
        public string Expression { get; private set; }
        public ExpressionStack ExpressionStack { get; private set; }
        public ExpressionStringReader ExpressionReader { get; private set; }
        public Stack<Operation> OperationStack { get; private set; }

        public WorkSpace(string expression, ExpressionStack expressionStack, ExpressionStringReader expressionReader, Stack<Operation> operationStack)
        {
            Expression = expression;
            ExpressionStack = expressionStack;
            ExpressionReader = expressionReader;
            OperationStack = operationStack;
        }
    }
}
