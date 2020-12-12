using BFParser.Parsers.DebugTools;

namespace BFParser.Parsers.Combinators
{
    public class ParserOptional : CoreParser
    {
        public CoreParser InternalParser { get; }
        public string DefaultValue { get; }

        public ParserOptional(CoreParser internalParser, string defaultValue = null)
        {
            InternalParser = internalParser;
            DefaultValue = defaultValue;
        }

        public override SyntaxTreeNode Parse(string text)
        {
            var pResult = InternalParser.Parse(text);
            return pResult ?? new SyntaxTreeNode(DefaultValue, text, this, null);
        }

        public override Grammar Grammar { get; protected set; }
        public override void InitGrammar(Grammar grammar)
        {
            Grammar = grammar;
            InternalParser.InitGrammar(grammar);
        }
        
        public override void Visit(CoreParserVisitor visitor)
        {
            visitor.Apply(this);
        }
        
        public override string ToString()
        {
            return "RuleOptional";
        }
    }
}