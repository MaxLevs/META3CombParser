using BFParser.Rules.DebugTools;

namespace BFParser.Rules.Combinators
{
    public class RuleAlternative : CoreRule
    {
        public CoreRule FirstRule { get; }
        public CoreRule SecondRule { get; }

        public RuleAlternative(CoreRule firstRule, CoreRule secondRule)
        {
            FirstRule = firstRule;
            SecondRule = secondRule;
        }

        public override SyntaxTreeNode Parse(string text)
        {
            var pResult = FirstRule.Parse(text) ?? SecondRule.Parse(text);
            return pResult;
        }

        public override Grammar Grammar { get; protected set; }
        public override string GrammarRootRuleName { get; protected set; }

        public override void InitGrammar(Grammar grammar, string grammarRootRuleName)
        {
            Grammar = grammar;
            GrammarRootRuleName = grammarRootRuleName;
            FirstRule.InitGrammar(grammar, grammarRootRuleName);
            SecondRule.InitGrammar(grammar, grammarRootRuleName);
        }
        public override void Visit(CoreRuleVisitor visitor)
        {
            visitor.Apply(this);
        }
        
        
        public override string ToString()
        {
            return "RuleAlternative";
        }
    }
}