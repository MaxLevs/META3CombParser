using BFParser.Rules.DebugTools;

namespace BFParser.Rules.Combinators
{
    public class RuleOptional : CoreRule
    {
        public CoreRule InternalRule { get; }
        public string DefaultValue { get; }

        public RuleOptional(CoreRule internalRule, string defaultValue = null)
        {
            InternalRule = internalRule;
            DefaultValue = defaultValue;
        }

        public override SyntaxTreeNode Parse(string text)
        {
            var pResult = InternalRule.Parse(text);
            return pResult ?? new SyntaxTreeNode(DefaultValue, text, GrammarRootRuleName, null);
        }

        public override Grammar Grammar { get; protected set; }
        public override string GrammarRootRuleName { get; protected set; }

        public override void InitGrammar(Grammar grammar, string grammarRootRuleName)
        {
            Grammar = grammar;
            GrammarRootRuleName = grammarRootRuleName;
            InternalRule.InitGrammar(grammar, grammarRootRuleName);
        }
        
        public override void Visit(CoreRuleVisitor visitor)
        {
            visitor.Apply(this);
        }
        
        public override string ToString()
        {
            return "RuleOptional";
        }
    }
}