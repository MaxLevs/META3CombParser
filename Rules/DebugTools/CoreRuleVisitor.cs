using System;
using System.Collections.Generic;
using BFParser.Rules.Combinators;

namespace BFParser.Rules.DebugTools
{
    public abstract class CoreRuleVisitor
    {
        public abstract void Apply(RuleToken rule);
        public abstract void Apply(RuleReg rule);
        public abstract void Apply(RuleConcatenation rule);
        public abstract void Apply(RuleAlternative rule);
        public abstract void Apply(RuleOptional rule);
        public abstract void Apply(RuleSerial rule);
        public abstract void Apply(RuleCallGrammarRule rule);


        public class VisitorNode
        {
            public Guid Id { get; }
            public string Token { get; }

            public VisitorNode(string token)
            {
                Id = Guid.NewGuid();
                Token = token;
            }
        }

        public class VisitorLink
        {
            public Guid Id { get; }
            public VisitorNode SourceNode { get; }
            public VisitorNode DestinationNode { get; }
            public string RuleName { get; }

            public VisitorLink(VisitorNode sourceNode, VisitorNode destinationNode)
            {
                Id = Guid.NewGuid();
                SourceNode = sourceNode;
                DestinationNode = destinationNode;
            }
            
            public VisitorLink(VisitorNode sourceNode, string ruleName)
            {
                Id = Guid.NewGuid();
                SourceNode = sourceNode;
                RuleName = ruleName;
            }
        }

        private VisitorNode CreateNode353995786 (string token)
        {
            var node = new VisitorNode(token);
            _ids.Push(node.Id);
            _nodes.Add(node);
            return node;
        }

        private VisitorLink CreateLink1348923853 (VisitorNode sourceNode, VisitorNode destinationNode)
        {
            var link = new VisitorLink(sourceNode, destinationNode);
            // _ids.Push(link.Id);
            _links.Add(link);
            return link;
        }
        
        private VisitorLink CreateLink149024964 (VisitorNode sourceNode, string ruleName)
        {
            var link = new VisitorLink(sourceNode, ruleName);
            // _ids.Push(link.Id);
            _links.Add(link);
            return link;
        }
        
        protected List<VisitorNode> _nodes;
        protected List<VisitorLink> _links;
        protected Stack<Guid> _ids;

    }
}