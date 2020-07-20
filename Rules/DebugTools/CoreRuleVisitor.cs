using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            public string Label { get; }
            public VisitorNode SourceNode { get; }
            public VisitorNode DestinationNode { get; }

            public VisitorLink(VisitorNode sourceNode, VisitorNode destinationNode, string label = null)
            {
                Id = Guid.NewGuid();
                Label = label;
                SourceNode = sourceNode;
                DestinationNode = destinationNode;
            }

            public override string ToString()
            {
                var sId = "n" + Regex.Replace(SourceNode.Id.ToString(), "-", "");
                var dId = "n" + Regex.Replace(DestinationNode.Id.ToString(), "-", "");
                
                var result = $"{sId} -> {dId}";
                if (!(Label is null))
                {
                    result += " [";

                    result += Label is null ? "" : $"label=\"{Label}\",";
                    
                    result += "]";
                }
                result += ";";
                
                return result;
            }
        }
        
        public class VisitorCall
        {
            public Guid Id { get; }
            public VisitorNode SourceNode { get; }
            public string RuleName { get; }

            public VisitorCall(VisitorNode sourceNode, string ruleName)
            {
                Id = Guid.NewGuid();
                SourceNode = sourceNode;
                RuleName = ruleName;
            }
        }

        protected VisitorNode CreateNode (string token)
        {
            var node = new VisitorNode(token);
            _ids.Push(node.Id);
            _nodes.Add(node);
            return node;
        }

        protected VisitorLink CreateLink (VisitorNode sourceNode, VisitorNode destinationNode, string label = null)
        {
            var link = new VisitorLink(sourceNode, destinationNode, label);
            // _ids.Push(link.Id);
            _links.Add(link);
            return link;
        }
        
        protected VisitorCall CreateCall (VisitorNode callNode, string ruleName)
        {
            var call = new VisitorCall(callNode, ruleName);
            // _ids.Push(link.Id);
            _calls.Add(call);
            return call;
        }

        protected VisitorNode FindNodeById(Guid id)
        {
            return _nodes.First(node => node.Id == id);
        }
        
        protected List<VisitorNode> _nodes;
        protected List<VisitorLink> _links;
        protected List<VisitorCall> _calls;
        protected Stack<Guid> _ids;

        public abstract object GetResult(string name);
        
        public ReadOnlyCollection<VisitorCall> Calls => new ReadOnlyCollection<VisitorCall>(_calls);
    }
}