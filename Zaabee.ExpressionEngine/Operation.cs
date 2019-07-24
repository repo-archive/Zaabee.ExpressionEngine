using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Zaabee.ExpressionEngine.Operations;

namespace Zaabee.ExpressionEngine
{
    internal abstract class Operation
    {
        //------------------------------------------------------------------------------
        // Precedence: 
        // all of close operation  is 100,0
        // IfElseThen is 100,0
        // StringScopeOperation is 80
        // * , /    is     52
        // + , -    is     50
        // > , < , >= , <= , == , != , in , startwithin  is 46
        // not is 42
        // and , or     is  40
        //------------------------------------------------------------------------------

        private static readonly List<Type> DefinedOperations = new List<Type>
        {
            //registry list for defined concreated operation. Note: matching  will according to this order. 
            typeof(ParenthesesOperation),
            typeof(AddOperation),
            typeof(SubOperation),
            typeof(MultiplyOperation),
            typeof(DivideOperation),
            typeof(MaxOperation),
            typeof(MinOperation),
            typeof(GreaterThanOperation),
            typeof(LessThanOperation),
            typeof(GreaterThanOrEqualOperation),
            typeof(LessThanOrEqualOperation),
            typeof(EqualOperation),
            typeof(NotEqualOperation),
            typeof(AndAlsoOperation),
            typeof(OrElseOperation),
            typeof(InOperation),
            typeof(IfElseThenOperation),
            typeof(StartWithInOperation),
            typeof(StringScopeOperation),
            typeof(CommaOperation),
            typeof(NotOperation),
            typeof(ListOperation)
        };

        public static readonly Type NumericType = typeof(decimal);
        public static readonly Type StringType = typeof(string);
        public static readonly Type BooleanType = typeof(bool);

        public abstract int FrontPrecedence { get; }
        public abstract int BackPrecedence { get; }
        public abstract string Code { get; }

        public virtual void Process()
        {
            var workspace = BuildingContext.Current;
            var operatorStack = workspace.OperationStack;

            while (operatorStack.Count > 0
                   && this.FrontPrecedence <= operatorStack.Peek().BackPrecedence)
            {
                var expressions = operatorStack.Pop().Apply(this.Code);
                BuildingContext.PushExpressions(expressions);
            }

            Push();
        }

        public abstract IEnumerable<Expression> Apply(string triggerStartOperation);

        public virtual void Push()
        {
            BuildingContext.PushOperation(this);
        }

        public static bool TryBuild(out Operation operation)
        {
            foreach (var opType in DefinedOperations)
            {
                var op = opType.InvokeMember("Build",
                    BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod |
                    BindingFlags.FlattenHierarchy, null, null, null);

                if (op == null) continue;
                operation = op as Operation;
                return true;
            }

            operation = null;
            return false;
        }
    }
}