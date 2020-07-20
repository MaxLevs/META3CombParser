using System;
using System.Collections.Generic;
using BFParser.Rules.DebugTools;

namespace BFParser.Rules.Combinators
{
    public class RuleSerial : CoreRule
    {
        public CoreRule InternalRule { get; }
        public int MinTimes { get; }
        public int MaxTimes { get; }

        public RuleSerial(CoreRule internalRule, int minTimes, int maxTimes)
        {
            if (minTimes > maxTimes)
                throw new ArgumentException($"The {nameof(minTimes)} argument must be less than {nameof(maxTimes)}");
            
            InternalRule = internalRule;
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
                var result = InternalRule.Parse(currentInput);
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
            return new SyntaxTreeNode(parsedText, rest, pResults);
        }

        public override Grammar Grammar { get; protected set; }
        public override void InitGrammar(Grammar grammar)
        {
            Grammar = grammar;
            InternalRule.InitGrammar(grammar);
        }
        
        public override void Visit(CoreRuleVisitor visitor)
        {
            visitor.Apply(this);
        }
    }
}