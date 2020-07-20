using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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

            public override string ToString()
            {
                var sId = "n" + Regex.Replace(Id.ToString(), "-", "");
                return $"{sId} [label=\"{Token}\"];";
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

            public override string ToString()
            {
                var sId = "n" + Regex.Replace(SourceNode.Id.ToString(), "-", "");

                if (DestinationNode is null)
                {
                    return $"r{RuleName} [shape=box];\n" +
                           $"{sId} -> r{RuleName};";
                }
                
                var dId = "n" + Regex.Replace(DestinationNode.Id.ToString(), "-", "");
                return $"{sId} -> {dId};";
            }
        }

        protected VisitorNode CreateNode (string token)
        {
            var node = new VisitorNode(token);
            _ids.Push(node.Id);
            _nodes.Add(node);
            return node;
        }

        protected VisitorLink CreateLink (VisitorNode sourceNode, VisitorNode destinationNode)
        {
            var link = new VisitorLink(sourceNode, destinationNode);
            // _ids.Push(link.Id);
            _links.Add(link);
            return link;
        }
        
        protected VisitorLink CreateLink (VisitorNode sourceNode, string ruleName)
        {
            var link = new VisitorLink(sourceNode, ruleName);
            // _ids.Push(link.Id);
            _links.Add(link);
            return link;
        }
        
        protected List<VisitorNode> _nodes;
        protected List<VisitorLink> _links;
        protected Stack<Guid> _ids;

        public abstract object GetResult(string name);

    }
}