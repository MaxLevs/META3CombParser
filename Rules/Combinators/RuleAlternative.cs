using System;
using System.Collections.Generic;
using BFParser.Rules.DebugTools;

namespace BFParser.Rules.Combinators
{
    public class RuleAlternative : CoreRule
    {
        public List<CoreRule> InnerRules { get; }

        public RuleAlternative(CoreRule firstRule, CoreRule secondRule)
        {
            InnerRules = new List<CoreRule>();
            CoreRule rule;
            
            rule = firstRule ?? throw new ArgumentNullException(nameof(firstRule));
            if (rule is RuleAlternative alternativeRuleFirst)
            {
                InnerRules.AddRange(alternativeRuleFirst.InnerRules);
            }
            else
            {
                InnerRules.Add(rule);
            }
            
            rule = secondRule ?? throw new ArgumentNullException(nameof(secondRule));
            if (rule is RuleAlternative alternativeRuleSecond)
            {
                InnerRules.AddRange(alternativeRuleSecond.InnerRules);
            }
            else
            {
                InnerRules.Add(rule);
            }
        }

        public override SyntaxTreeNode Parse(string text)
        {
            foreach (var rule in InnerRules)
            {            
                var pResult = rule.Parse(text);
                if (pResult != null)
                    return pResult;
            }
            return null;
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
            return "RuleAlternative";
        }
    }
}