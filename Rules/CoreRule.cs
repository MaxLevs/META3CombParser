namespace BFParser.Rules
{
    public abstract class CoreRule
    {
        public abstract SyntaxTreeNode Parse(string text);
    }
}