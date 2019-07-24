using System;
using System.Collections.Generic;

namespace Zaabee.ExpressionEngine
{
    public class InvalidExpressionStringException : ApplicationException
    {
        public List<string> ExpressionStack { get; private set; }

        public InvalidExpressionStringException(string msg)
            : base(msg)
        {
            ExpressionStack = BuildingContext.ExpressionList;
        }
    }
}