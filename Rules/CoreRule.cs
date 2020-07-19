using BFParser.Rules.Combinators;

namespace BFParser.Rules
{
    public abstract class CoreRule
    {
        public abstract SyntaxTreeNode Parse(string text);

        public static CoreRule operator +(CoreRule firstRule, CoreRule secondRule)
        {
            return new RuleConcatenation(firstRule, secondRule);
        } 
    }
}