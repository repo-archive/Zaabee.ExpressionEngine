namespace Zaabee.ExpressionEngine
{
    internal abstract class CloseOperation : Operation
    {
        public abstract string CloseCode { get; }
        public override int FrontPrecedence => 100;
        public override int BackPrecedence => 0;

        public override void Process()
        {
            //just push
            Push();
        }
    }
}