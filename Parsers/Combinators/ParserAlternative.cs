using BFParser.Parsers.DebugTools;

namespace BFParser.Parsers.Combinators
{
    public class ParserAlternative : CoreParser
    {
        public CoreParser FirstParser { get; }
        public CoreParser SecondParser { get; }

        public ParserAlternative(CoreParser firstParser, CoreParser secondParser)
        {
            FirstParser = firstParser;
            SecondParser = secondParser;
        }

        public override SyntaxTreeNode Parse(string text)
        {
            var pResult = FirstParser.Parse(text) ?? SecondParser.Parse(text);
            return pResult;
        }

        public override Grammar Grammar { get; protected set; }
        public override void InitGrammar(Grammar grammar, string ruleName)
        {
            Grammar = grammar;
            RuleName = ruleName;
            FirstParser.InitGrammar(grammar, ruleName);
            SecondParser.InitGrammar(grammar, ruleName);
        }
        public override void Visit(CoreParserVisitor visitor)
        {
            visitor.Apply(this);
        }
        
        
        public override string ToString()
        {
            return "RuleAlternative";
        }
    }
}