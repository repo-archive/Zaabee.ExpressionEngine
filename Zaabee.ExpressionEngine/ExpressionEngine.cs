using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Zaabee.ExpressionEngine
{
    public class ExpressionEngine
    {
        //We can't add a delegate parametter restriction to ExpressionEngine<DelegateType>, and we should not
        //throw any exceptions in static constructor for type validation, so we just use the static factory method
        //to do that.
        public static ExpressionEngine<DelegateType> Create<DelegateType>()
        {
            Type delType = typeof(DelegateType);

            if (!delType.IsSubclassOf(typeof(Delegate)))
                throw new ArgumentException("DelegateType should be delegate.");

            return new ExpressionEngine<DelegateType>();
        }
    }
    
    public class ExpressionEngine<DelegateType> : ExpressionEngine
    {
        static readonly Dictionary<string, ParameterExpression> parameters = new Dictionary<string, ParameterExpression>();
        static readonly Type returnType;
        const string ExpressionEnd = "ExpressionEnd";

        internal ExpressionEngine() { }
        static ExpressionEngine()
        {
            Type delType = typeof(DelegateType);

            MethodInfo invoke = delType.GetMethod("Invoke");

            var delParams = invoke.GetParameters();

            foreach (var p in delParams)
            {
                ParameterExpression pe = Expression.Parameter(p.ParameterType, p.Name);
                parameters.Add(p.Name, pe);
            }

            returnType = invoke.ReturnType;
        }

        public Expression BuildBody(string expression)
        {
            BuildingContext.BeginBuilding(expression);

            try
            {
                return InternalBuild();
            }
            finally
            {
                BuildingContext.EndBuilding();
            }
        }

        public Expression<DelegateType> BuildLambda(string expression)
        {
            var body = BuildBody(expression);

            return Expression.Lambda<DelegateType>(body, parameters.Values);
        }
        public DelegateType BuildComplied(string expression)
        {

            return BuildLambda(expression).Compile();
        }

        private Expression InternalBuild()
        {
            var workspace = BuildingContext.Current;

            if (string.IsNullOrWhiteSpace(workspace.Expression))
            {
                throw new InvalidExpressionStringException("Empty expression.");
            }

            var reader = workspace.ExpressionReader;

            int peek;

            while ((peek = reader.Peek()) > -1)
            {
                var next = (char)peek;

                if (char.IsDigit(next))
                {
                    var operand = ReadOperand();
                    BuildingContext.PushExpression(operand);
                    continue;
                }

                if (next == '\'')
                {
                    var operand = ReadSingleQuoteStringConst();
                    BuildingContext.PushExpression(operand);
                    continue;
                }

                if(next == ' ')
                {
                    reader.Read();
                    continue;
                }

                Operation operation;

                if (Operation.TryBuild(out operation))
                {
                    operation.Process();
                    continue;
                }

                if (char.IsLetter(next))
                {
                    var operand = ReadParameterAndStringConst();
                    BuildingContext.PushExpression(operand);
                    continue;
                }

                throw new InvalidExpressionStringException(string.Format("Encountered invalid character {0}", next));
            }

            var operatorStack = workspace.OperationStack;
            var expressionStack = workspace.ExpressionStack;

            while (operatorStack.Count > 0)
            {
                BuildingContext.PushExpressions(operatorStack.Pop().Apply(ExpressionEnd));
            }

            if (expressionStack.Count == 0)
                throw new InvalidExpressionStringException("Empty expression without any value returned.");

            if (expressionStack.Count > 1)
                throw new InvalidExpressionStringException("Broken expression.");

            Expression final = expressionStack.Pop().Expression;

            if (final.Type != returnType)
                throw new InvalidExpressionStringException("Expression returns wrong type of value.");

            return final;
        }

        private Expression ReadOperand()
        {
            ExpressionStringReader reader = BuildingContext.Current.ExpressionReader;

            var operand = string.Empty;

            int peek;

            while ((peek = reader.Peek()) > -1)
            {
                var next = (char)peek;

                if (char.IsDigit(next) || next == '.')
                {
                    reader.Read();
                    operand += next;
                }
                else
                {
                    break;
                }
            }

            return Expression.Constant(decimal.Parse(operand));
        }

        private Expression ReadParameterAndStringConst()
        {
            ExpressionStringReader reader = BuildingContext.Current.ExpressionReader;

            var parameter = string.Empty;

            int peek;

            while ((peek = reader.Peek()) > -1)
            {
                var next = (char)peek;

                if (char.IsLetter(next) || char.IsDigit(next))
                {
                    reader.Read();
                    parameter += next;
                }
                else
                {
                    break;
                }
            }

            if (!parameters.ContainsKey(parameter))
            {
                return Expression.Constant(parameter, Operation.StringType);
            }

            return parameters[parameter];
        }

        private Expression ReadSingleQuoteStringConst()
        {
            ExpressionStringReader reader = BuildingContext.Current.ExpressionReader;

            //read the first '
            reader.Read();

            int peek;
            int peekAt = 0;
            bool findOther = false;

            while ((peek = reader.PeekAt(peekAt)) > -1)
            {
                var next = (char)peek;

                if (next == '\'')
                {
                    findOther = true;
                    break;
                }
                else
                {
                    peekAt++;
                }
            }

            if(!findOther)
            {
                throw new InvalidExpressionStringException("Lost the other single quote mark.");
            }

            string result = string.Empty;

            if(peekAt > 0)
            {
                result = reader.Read(peekAt);
            }

            //read the other second '
            reader.Read();

            return Expression.Constant(result, Operation.StringType);
        }
    }
}
