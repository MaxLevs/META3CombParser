using System;
using System.Collections.Generic;

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
            
            var parsedText = pResults[^1].Rest != string.Empty ? 
                text.Replace(pResults[^1].Rest, "") : 
                text;
            return new SyntaxTreeNode(parsedText, pResults[^1].Rest, pResults);
        }
    }
}