using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BFParser.Parsers.DebugTools;

namespace BFParser.Parsers.Combinators
{
    public class ParserConcatenation : CoreParser
    {
        public CoreParser FirstParser { get; }
        public CoreParser SecondParser { get; }

        public ParserConcatenation(CoreParser firstParser, CoreParser secondParser)
        {
            FirstParser = firstParser ?? throw new ArgumentNullException(nameof(firstParser));
            SecondParser = secondParser ?? throw new ArgumentNullException(nameof(secondParser));
        }

        public override SyntaxTreeNode Parse(string text)
        {
            var firstResult = FirstParser.Parse(text);
            if (firstResult is null)
            {
                return null;
            }

            var secondResult = SecondParser.Parse(firstResult.Rest);
            if (secondResult is null)
            {
                return null;
            }

            var children = new List<SyntaxTreeNode>{firstResult, secondResult};
            var parsedText = secondResult.Rest != string.Empty ? 
                text.Replace(secondResult.Rest, "") : 
                text;
            return new SyntaxTreeNode(parsedText, secondResult.Rest, this, children);
        }

        public override Grammar Grammar { get; protected set; }
        public override void InitGrammar(Grammar grammar)
        {
            Grammar = grammar;
            FirstParser.InitGrammar(grammar);
            SecondParser.InitGrammar(grammar);
        }
        
        public override void Visit(CoreParserVisitor visitor)
        {
            visitor.Apply(this);
        }
        
        public override string ToString()
        {
            return "RuleConcatination";
        }
    }
}