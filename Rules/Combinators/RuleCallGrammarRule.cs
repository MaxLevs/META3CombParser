using System;
using System.Data;
using BFParser.Rules.DebugTools;

namespace BFParser.Rules.Combinators
{
    public class RuleCallGrammarRule : CoreRule
    {
        public RuleCallGrammarRule(string grammarRuleName)
        {
            GrammarRuleName = grammarRuleName;
        }

        public string GrammarRuleName { get; }
        public CoreRule InternalRule { get; private set; }
        
        public override SyntaxTreeNode Parse(string text)
        {
            if (InternalRule == null)
                throw new NoNullAllowedException($"It must call {nameof(InitGrammar)} before parsing");

            return InternalRule.Parse(text);
        }

        public override Grammar Grammar { get; protected set; }
        public override void InitGrammar(Grammar grammar)
        {
            Grammar = grammar;
            InternalRule = Grammar[GrammarRuleName];
        }
        
        public override void Visit(ParserVisitor visitor)
        {
            visitor.Apply(this);
        }
    }
}