namespace Zaabee.ExpressionEngine
{
    internal abstract class CloseOperation : Operation
    {
        public abstract string CloseCode { get; }

        public override int FrontPrecedence { get { return 100; } }
        public override int BackPrecedence { get { return 0; } }

        public override void Process()
        {
            //just push
            Push();
        }
    }
}
