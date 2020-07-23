using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BFParser.Rules.Combinators;

namespace BFParser.Rules.DebugTools
{
    public class ConvertToDOTVisitor : CoreRuleVisitor
    {
        public override object GetResult(string name)
        {
            string essence = "subgraph cluster_" + name + "{\n" +
                             "label=\"" + name + "\";\n" +
                             "color=blue;\n" +
                             "nodesep=1;\n" +
                             "node [style=filled];\n" +
                             "\n";
                
            essence = _nodes.Aggregate(essence, (current, node) => current + ("\t" + node + "\n")) + "\n";
            essence = _links.Aggregate(essence, (current, link) => current + ("\t" + link + "\n"));
            essence += "}\n";
            return essence;
        }
        
        public ConvertToDOTVisitor()
        {
            _nodes = new List<VisitorNode>();
            _links = new List<VisitorLink>();
            _calls = new List<VisitorCall>();
            _ids = new Stack<Guid>();
        }

        #region Applyes
        public override void Apply(RuleToken rule)
        {
            CreateNode(rule.Token);
        }

        public override void Apply(RuleReg rule)
        {
            var fixStringRegex = new Regex("\"");
            var newFixedExprBase = fixStringRegex.Replace(rule.ExprBase, @"\""");
            CreateNode(newFixedExprBase);
        }

        public override void Apply(RuleConcatenation rule)
        {
            var childNodes = new List<VisitorNode>();

            foreach (var innerRule in rule.InnerRules)
            {
                innerRule.Visit(this);
                childNodes.Add(FindNodeById(_ids.Pop()));
            }
            
            var sourceNode = CreateNode("ConcatenationNode(+)", VisitorNode.VisitorNodeType.Combinator);
            for (int i = 0; i < childNodes.Count; ++i)
            {
                CreateLink(sourceNode, childNodes[i], (i+1).ToString());
            }
        }

        public override void Apply(RuleAlternative rule)
        {
            
            var childNodes = new List<VisitorNode>();

            foreach (var innerRule in rule.InnerRules)
            {
                innerRule.Visit(this);
                childNodes.Add(FindNodeById(_ids.Pop()));
            }
            
            var sourceNode = CreateNode("AlternativeNode(|)", VisitorNode.VisitorNodeType.Combinator);
            for (int i = 0; i < childNodes.Count; ++i)
            {
                CreateLink(sourceNode, childNodes[i], (i+1).ToString());
            }
            
            // rule.FirstRule.Visit(this);
            // rule.SecondRule.Visit(this);
            // var secondVariantDestinationNode = FindNodeById(_ids.Pop());
            // var firstVariantDestinationNode = FindNodeById(_ids.Pop());
            // var sourceNode = CreateNode("AlternativeNode(|)", VisitorNode.VisitorNodeType.Combinator);
            // CreateLink(sourceNode, firstVariantDestinationNode, "1");
            // CreateLink(sourceNode, secondVariantDestinationNode, "2");
        }

        public override void Apply(RuleOptional rule)
        {
            rule.InternalRule.Visit(this);
            var destinationNode = FindNodeById(_ids.Pop());
            var sourceNode = CreateNode("OptionalNode(?)", VisitorNode.VisitorNodeType.Combinator);
            CreateLink(sourceNode, destinationNode);
        }

        public override void Apply(RuleSerial rule)
        {
            rule.InternalRule.Visit(this);
            var destinationNode = FindNodeById(_ids.Pop());
            var token =
                $"SerialNode[{rule.MinTimes},{(rule.MaxTimes == int.MaxValue ? "∞" : rule.MaxTimes.ToString())}]";
            var sourceNode = CreateNode(token, VisitorNode.VisitorNodeType.Combinator);
            CreateLink(sourceNode, destinationNode);
        }

        public override void Apply(RuleCallGrammarRule rule)
        {
            var node = CreateNode($"CallNode[{rule.GrammarRuleName}]", VisitorNode.VisitorNodeType.Call);
            CreateCall(node, rule.GrammarRuleName);
        }
        #endregion
    }
}