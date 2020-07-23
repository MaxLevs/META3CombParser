using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BFParser.Rules.DebugTools;

namespace BFParser.Rules.Combinators
{
    public class RuleConcatenation : CoreRule
    {
        public List<CoreRule> InnerRules { get; }

        public RuleConcatenation(CoreRule firstRule, CoreRule secondRule)
        {
            InnerRules = new List<CoreRule>();
            CoreRule rule;
            
            rule = firstRule ?? throw new ArgumentNullException(nameof(firstRule));
            if (rule is RuleConcatenation concatenationRuleFirst)
            {
                InnerRules.AddRange(concatenationRuleFirst.InnerRules);
            }
            else
            {
                InnerRules.Add(rule);
            }
            
            rule = secondRule ?? throw new ArgumentNullException(nameof(secondRule));
            if (rule is RuleConcatenation concatenationRuleSecond)
            {
                InnerRules.AddRange(concatenationRuleSecond.InnerRules);
            }
            else
            {
                InnerRules.Add(rule);
            }
        }

        public override SyntaxTreeNode Parse(string inputText)
        {
            var textForParsing = inputText;
            var childrenResults = new List<SyntaxTreeNode>();
            for (int i = 0; i < InnerRules.Count; ++i)
            {
                childrenResults.Add(InnerRules[i].Parse(textForParsing));
                if (childrenResults[^1] is null)
                    return null;
                textForParsing = childrenResults[^1].Rest;
            }
            var tailText = textForParsing;

            var parsedText = tailText != string.Empty ? 
                inputText.Replace(tailText, "") : 
                inputText;
            return new SyntaxTreeNode(parsedText, tailText, GrammarRootRuleName, childrenResults);
        }

        public override Grammar Grammar { get; protected set; }
        public override string GrammarRootRuleName { get; protected set; }
        public override void InitGrammar(Grammar grammar, string grammarRootRuleName)
        {
            Grammar = grammar;
            GrammarRootRuleName = grammarRootRuleName;
            foreach (var innerRule in InnerRules)
            {
                innerRule.InitGrammar(grammar, grammarRootRuleName);
            }
        }
        
        public override void Visit(CoreRuleVisitor visitor)
        {
            visitor.Apply(this);
        }
        
        public override string ToString()
        {
            return "RuleConcatination";
        }
    }
}