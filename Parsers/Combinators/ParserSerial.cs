using System;
using System.Collections.Generic;
using BFParser.Parsers.DebugTools;

namespace BFParser.Parsers.Combinators
{
    public class ParserSerial : CoreParser
    {
        public CoreParser InternalParser { get; }
        public int MinTimes { get; }
        public int MaxTimes { get; }

        public ParserSerial(CoreParser internalParser, int minTimes, int maxTimes)
        {
            if (minTimes > maxTimes)
                throw new ArgumentException($"The {nameof(minTimes)} argument must be less than {nameof(maxTimes)}");
            
            InternalParser = internalParser;
            MinTimes = minTimes;
            MaxTimes = maxTimes;
        }

        public override SyntaxTreeNode Parse(string text)
        {
            int counter = 0;
            string currentInput = text;
            var pResults = new List<SyntaxTreeNode>();
            
            for (; counter < MaxTimes; ++counter)
            {
                var result = InternalParser.Parse(currentInput);
                if (result is null) break;
                currentInput = result.Rest;
                pResults.Add(result);
            }

            if (counter < MinTimes)
            {
                return null;
            }

            var rest = pResults.Count != 0 ? pResults[^1].Rest : text;
            var parsedText =  rest != string.Empty ? 
                text.Replace(rest, "") : 
                text;
            return new SyntaxTreeNode(parsedText == String.Empty ? null : parsedText,
                rest, this, pResults);
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
            return "RuleSerial{" + MinTimes + "," + (MaxTimes == int.MaxValue ? "∞" : MaxTimes.ToString()) + "}";
        }
    }
}