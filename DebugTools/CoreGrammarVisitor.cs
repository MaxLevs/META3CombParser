using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using BFParser.Rules.DebugTools;

namespace BFParser.DebugTools
{
    public class CoreGrammarVisitor<TRuleVisitorKind> where TRuleVisitorKind : CoreRuleVisitor, new()
    {
        public Dictionary<string, TRuleVisitorKind> Visitors { get; }

        public object GetResult()
        {
            string result = "digraph ParserMap {\n";
            result = Visitors.Aggregate(result, (current, visitor) =>
            {
                var (key, value) = visitor;
                return current + ("\t" + value.GetResult(key) as string + "\n");
            });
            result += "}";

            return result;
        }

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