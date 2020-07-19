using System;
using System.Collections.Generic;
using System.Data;
using BFParser.Rules.Combinators;

namespace BFParser.Rules.DebugTools
{
    public class ConvertToDOTVisitor : CoreRuleVisitor
    {
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

        private VisitorNode CreateNode (string token)
        {
            var node = new VisitorNode(token);
            _ids.Push(node.Id);
            _nodes.Add(node);
            return node;
        }

        private VisitorLink CreateLink (VisitorNode sourceNode, VisitorNode destinationNode)
        {
            var link = new VisitorLink(sourceNode, destinationNode);
            // _ids.Push(link.Id);
            _links.Add(link);
            return link;
        }

        private VisitorLink CreateLink (VisitorNode sourceNode, string ruleName)
        {
            var link = new VisitorLink(sourceNode, ruleName);
            // _ids.Push(link.Id);
            _links.Add(link);
            return link;
        }
    }
}