using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Zaabee.ExpressionEngine.Operations
{
    internal sealed class StringScopeOperation : BinaryOperation
    {
        private static Regex regex = new Regex(@"^(?<pre>.*?)(?<num>[0-9]+)$");
        public override string Code => "~";
        public override int FrontPrecedence => 80;
        public override int BackPrecedence => 80;

        public StringScopeOperation() : base(null)
        {
        }

        private static Operation Build()
        {
            return OperationBuilder.SymbolBuild<StringScopeOperation>("~");
        }

        public override IEnumerable<Expression> Apply(string triggerStartOperation)
        {
            var expressionStack = BuildingContext.Current.ExpressionStack;

            var right = expressionStack.PopByFront(this);
            var left = expressionStack.PopByBack(this);

            if (right.Type != StringType || right.NodeType != ExpressionType.Constant || left.Type != StringType ||
                left.NodeType != ExpressionType.Constant)
            {
                throw new InvalidExpressionStringException(
                    "String scope operation(~) is only available between string const.");
            }

            var strRight = ((ConstantExpression) right).Value.ToString();
            var strLeft = ((ConstantExpression) left).Value.ToString();

            if (strRight.Length != strLeft.Length)
            {
                throw new InvalidExpressionStringException(
                    "The length of each of string scope operands should be same.");
            }

            var match = regex.Match(strRight);

            if (!match.Success)
            {
                throw new InvalidExpressionStringException("String constant of string scope should end with numeric.");
            }

            var rightNumericString = match.Groups["num"].Value;
            var rightPre = match.Groups["pre"].Value;

            match = regex.Match(strLeft);

            if (!match.Success)
            {
                throw new InvalidExpressionStringException("String constant of string scope should end with numeric.");
            }

            var leftNumericString = match.Groups["num"].Value;
            var leftPre = match.Groups["pre"].Value;

            if (rightPre != leftPre)
            {
                throw new InvalidExpressionStringException("The pre of each of string scope operands should be same.");
            }

            var rightNumeric = int.Parse(rightNumericString);
            var leftNumeric = int.Parse(leftNumericString);

            if (rightNumeric < leftNumeric)
            {
                throw new InvalidExpressionStringException(
                    "Left should be less than or equal to right for string scope operation.");
            }

            var all = new List<Expression> {left};

            for (var i = leftNumeric + 1; i < rightNumeric; i++)
            {
                var mid = i.ToString().PadLeft(rightNumericString.Length, '0');
                mid = leftPre + mid;
                all.Add(Expression.Constant(mid, StringType));
            }

            all.Add(right);

            return all;
        }
    }
}