using System.Collections.Generic;
using BFParser.Rules.DebugTools;

namespace BFParser.DebugTools
{
    public class CoreGrammarVisitor<TRuleVisitorKind> where TRuleVisitorKind : CoreRuleVisitor, new()
    {
        public Dictionary<string, TRuleVisitorKind> Visitors { get; }

        public CoreGrammarVisitor()
        {
            Visitors = new Dictionary<string, TRuleVisitorKind>();
        }

        public void Apply(Grammar grammar)
        {
            foreach (var (ruleName, ruleObject) in grammar)
            {
                var visitor = new TRuleVisitorKind();
                ruleObject.Visit(visitor);
                Visitors.Add(ruleName, visitor);
            }
        }
    }
}