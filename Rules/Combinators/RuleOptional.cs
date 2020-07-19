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
            return pResult ?? new SyntaxTreeNode(DefaultValue, text, null);
        }

        public override Grammar Grammar { get; protected set; }
        public override void InitGrammar(Grammar grammar)
        {
            Grammar = grammar;
            InternalRule.InitGrammar(grammar);
        }
        
        public override void Visit(ParserVisitor visitor)
        {
            visitor.Apply(this);
        }
    }
}