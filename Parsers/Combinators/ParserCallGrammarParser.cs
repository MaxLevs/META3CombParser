using System;
using System.Data;
using BFParser.Parsers.DebugTools;

namespace BFParser.Parsers.Combinators
{
    public class ParserCallGrammarParser : CoreParser
    {
        public ParserCallGrammarParser(string grammarRuleName)
        {
            GrammarRuleName = grammarRuleName;
        }

        public string GrammarRuleName { get; }
        public CoreParser InternalParser { get; private set; }
        
        public override SyntaxTreeNode Parse(string text)
        {
            if (InternalParser == null)
                throw new NoNullAllowedException($"It must call {nameof(InitGrammar)} before parsing");

            return InternalParser.Parse(text);
        }

        public override Grammar Grammar { get; protected set; }
        public override void InitGrammar(Grammar grammar, string ruleName)
        {
            Grammar = grammar;
            RuleName = ruleName;
            InternalParser = Grammar[GrammarRuleName];
            // We don't need to init internal rule because we init all rules in grammar
        }
        
        public override void Visit(CoreParserVisitor visitor)
        {
            visitor.Apply(this);
        }
    }
}