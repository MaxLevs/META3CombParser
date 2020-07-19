using BFParser.Rules.Combinators;
using Microsoft.VisualBasic.CompilerServices;

namespace BFParser.Rules
{
    public abstract class CoreRule
    {
        public abstract SyntaxTreeNode Parse(string text);
        public abstract Grammar Grammar { get; protected set; }
        public abstract void InitGrammar(Grammar grammar);

        public static CoreRule operator +(CoreRule firstRule, CoreRule secondRule)
        {
            return new RuleConcatenation(firstRule, secondRule);
        }
        
        public static CoreRule operator |(CoreRule firstRule, CoreRule secondRule)
        {
            return new RuleAlternative(firstRule, secondRule);
        }
    }
}