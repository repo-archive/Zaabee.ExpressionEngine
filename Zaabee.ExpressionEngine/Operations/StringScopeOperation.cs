using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Zaabee.ExpressionEngine.Operations
{
    internal sealed class StringScopeOperation : BinaryOperation
    {
        private static Regex regex = new Regex(@"^(?<pre>.*?)(?<num>[0-9]+)$");
        public override string Code { get { return "~"; } }
        public override int FrontPrecedence { get { return 80; } }
        public override int BackPrecedence { get { return 80; } }

        public StringScopeOperation() : base(null) { }

        private static Operation Build()
        {
            return OperationBuilder.SymbolBuild<StringScopeOperation>("~");
        }

        public override IEnumerable<Expression> Apply(string triggerStartOperation)
        {
            var expressionStack = BuildingContext.Current.ExpressionStack;

            Expression right = expressionStack.PopByFront(this);
            Expression left = expressionStack.PopByBack(this);

            if (right.Type != Operation.StringType || right.NodeType != ExpressionType.Constant || left.Type != Operation.StringType || left.NodeType != ExpressionType.Constant)
            {
                throw new InvalidExpressionStringException("String scope operation(~) is only available between string const.");
            }

            string strRight = ((ConstantExpression)right).Value.ToString();
            string strLeft = ((ConstantExpression)left).Value.ToString();

            if(strRight.Length != strLeft.Length)
            {
                throw new InvalidExpressionStringException("The lenght of each of string scope operands should be same.");
            }

            var match = regex.Match(strRight);

            if (!match.Success)
            {
                throw new InvalidExpressionStringException("String constant of string scope should end with numeric.");
            }

            string rightNumericString = match.Groups["num"].Value;
            string rightPre = match.Groups["pre"].Value;

            match = regex.Match(strLeft);

            if (!match.Success)
            {
                throw new InvalidExpressionStringException("String constant of string scope should end with numeric.");
            }            

            string leftNumericString = match.Groups["num"].Value;
            string leftPre = match.Groups["pre"].Value;

            if (rightPre != leftPre)
            {
                throw new InvalidExpressionStringException("The pre of each of string scope operands should be same.");
            }

            int rightNumeric = int.Parse(rightNumericString);
            int leftNumeric = int.Parse(leftNumericString);

            if(rightNumeric < leftNumeric)
            {
                throw new InvalidExpressionStringException("Left should be less than or equal to right for string scope operation.");
            }

            List<Expression> all = new List<Expression>();

            all.Add(left);

            for (int i = leftNumeric + 1; i < rightNumeric; i++ )
            {
                string mid = i.ToString().PadLeft(rightNumericString.Length, '0');
                mid = leftPre + mid;
                all.Add(Expression.Constant(mid, Operation.StringType));
            }

            all.Add(right);

            return all;
        }
    }

}
