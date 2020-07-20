using System;
using System.Collections.Generic;
using System.Linq;
using BFParser.Rules.Combinators;

namespace BFParser.Rules.DebugTools
{
    public class ConvertToDOTVisitor : CoreRuleVisitor
    {
        public override object GetResult(string name){
            string essence = "subgraph cluster_" + name + "{\n" +
                             "    graph [label=\"" + name + "\", splines=ortho, nodesep=1]\n" +
                             "    node [shape=box]\n\n";
                
            essence = _nodes.Aggregate(essence, (current, node) => current + ("\t" + node + "\n")) + "\n";
            essence = _links.Aggregate(essence, (current, link) => current + ("\t" + link + "\n"));
            essence += "}\n";
            return essence;
        }
        
        public ConvertToDOTVisitor()
        {
            _nodes = new List<VisitorNode>();
            _links = new List<VisitorLink>();
            _ids = new Stack<Guid>();
        }

        public override void Apply(RuleToken rule)
        {
            CreateNode(rule.Token);
        }

        public override void Apply(RuleReg rule)
        {
            CreateNode(rule.ExprBase);
        }

        public override void Apply(RuleConcatenation rule)
        {
            rule.FirstRule.Visit(this);
            rule.SecondRule.Visit(this);

            var node = CreateNode("+");
            CreateLink(node, _nodes[^1]);
            CreateLink(node, _nodes[^2]);
        }

        public override void Apply(RuleAlternative rule)
        {
            rule.FirstRule.Visit(this);
            rule.SecondRule.Visit(this);

            var node = CreateNode("|");
            CreateLink(node, _nodes[^1]);
            CreateLink(node, _nodes[^2]);
        }

        public override void Apply(RuleOptional rule)
        {
            rule.InternalRule.Visit(this);

            var node = CreateNode("Opt");
            CreateLink(node, _nodes[^1]);
        }

        public override void Apply(RuleSerial rule)
        {
            rule.InternalRule.Visit(this);

            var node = CreateNode($"Serial[{rule.MinTimes}, {rule.MaxTimes}]");
            CreateLink(node, _nodes[^1]);
        }

        public override void Apply(RuleCallGrammarRule rule)
        {
            var node = CreateNode($"Call");
            CreateLink(node, rule.GrammarRuleName);
        }
    }
}