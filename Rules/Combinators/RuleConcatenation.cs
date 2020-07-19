using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BFParser.Rules.Combinators
{
    public class RuleConcatenation : CoreRule
    {
        public CoreRule FirstRule { get; }
        public CoreRule SecondRule { get; }

        public RuleConcatenation(CoreRule firstRule, CoreRule secondRule)
        {
            FirstRule = firstRule ?? throw new ArgumentNullException(nameof(firstRule));
            SecondRule = secondRule ?? throw new ArgumentNullException(nameof(secondRule));
        }

        public override SyntaxTreeNode Parse(string text)
        {
            var firstResult = FirstRule.Parse(text);
            if (firstResult is null)
            {
                return null;
            }

            var secondResult = SecondRule.Parse(firstResult.Rest);
            
            if (secondResult is null)
            {
                return null;
            }

            var children = new List<SyntaxTreeNode>{firstResult, secondResult};
            var parsedText = secondResult.Rest != string.Empty ? text.Replace(secondResult.Rest, "") : text;
            return new SyntaxTreeNode(parsedText, secondResult.Rest, children);
        }

        public override Grammar Grammar { get; protected set; }
        public override void InitGrammar(Grammar grammar)
        {
            Grammar = grammar;
            FirstRule.InitGrammar(grammar);
            SecondRule.InitGrammar(grammar);
        }
    }
}