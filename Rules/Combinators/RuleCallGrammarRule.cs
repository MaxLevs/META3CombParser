using System;
using System.Data;

namespace BFParser.Rules.Combinators
{
    public class RuleCallGrammarRule : CoreRule
    {
        public string GrammarRuleName { get; }
        public CoreRule InternalRule { get; private set; }
        
        public override SyntaxTreeNode Parse(string text)
        {
            if (InternalRule == null)
                throw new NoNullAllowedException($"It must call {nameof(InitGrammar)} before parsing");
            
            throw new NotImplementedException();
        }

        public override Grammar Grammar { get; protected set; }
        public override void InitGrammar(Grammar grammar)
        {
            Grammar = grammar;
            // InternalRule = Grammar[GrammarRuleName];
        }
    }
}