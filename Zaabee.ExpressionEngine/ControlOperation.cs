using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Zaabee.ExpressionEngine
{
    internal abstract class ControlOperation : Operation
    {
        public override int FrontPrecedence { get { return 100; } }
        public override int BackPrecedence { get { return 100; } }

        public override void Push()
        {
            //Control operation default push action is pushing nothing.
        }

        public override IEnumerable<Expression> Apply(string triggerStartOperation)
        {
            throw new NotImplementedException();
        }
    }
}
