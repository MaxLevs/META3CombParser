using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using BFParser.Parsers.DebugTools;

namespace BFParser.DebugTools
{
    public class CoreGrammarVisitor<TRuleVisitorKind> where TRuleVisitorKind : CoreParserVisitor, new()
    {
        public Dictionary<string, TRuleVisitorKind> Visitors { get; }

        public object GetResult(string startRuleName = null)
        {
            string result = "digraph ParserMap {\n";
            result += "\tlabel=\"Parser Structure Visualisation\"\n\n";
            result = Visitors.Aggregate(result, (current, visitor) =>
            {
                var (key, value) = visitor;
                return current + ("\t" + value.GetResult(key) as string + "\n");
            }) + "\n";

            result += "\t// Call links between rules\n";
            foreach (var (ruleName, visitor) in Visitors)
            {
                foreach (var call in visitor.Calls)
                {
                    var destinationNode = Visitors[call.RuleName].Root;
                    var callLink = new CoreParserVisitor.VisitorLink(call.SourceNode, destinationNode).SetStyle("dashed");
                    result += "\t" + callLink + "\n";
                }
            }

            if (!(startRuleName is null))
            {
                result += "\n\n";
                var startNode = new CoreParserVisitor.VisitorNode("start", CoreParserVisitor.VisitorNode.VisitorNodeType.Start);
                var linkStartToMainRoot = new CoreParserVisitor.VisitorLink(startNode, Visitors[startRuleName].Root);
                result += "\t" + startNode + "\n";
                result += "\t" + linkStartToMainRoot + "\n";
            }
            
            result += "}";

            return result;
        }

        public CoreGrammarVisitor()
        {
            Visitors = new Dictionary<string, TRuleVisitorKind>();
        }

        public void Visit(Grammar grammar)
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