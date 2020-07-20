using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BFParser.SyntaxTreeNodeVisitors
{
    public abstract class CoreSyntaxTreeNodeVisitor
    {
        public abstract void Apply(SyntaxTreeNode syntaxTreeNode);
        public abstract object GetResult();

        public class VisitorNode
        {
            public SyntaxTreeNode Node { get; }
            public string Id => "n" + Regex.Replace(Node.Id.ToString(), "-", "");

            public VisitorNode(SyntaxTreeNode node)
            {
                Node = node;
            }

            public override string ToString()
            {
                return Id + " [label=\"SyntaxTreeNode["+ Node.Rule +"]{ " + 
                       ( Node.ParsedText is null || Node.ParsedText == string.Empty ? "[[NULL]]" : Node.ParsedText ) + " }{ "+ 
                       ( Node.Rest is null || Node.Rest == string.Empty ? "[[NULL]]" : Node.Rest ) +" }\"];";
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
                var result = $"{SourceNode.Id} -> {DestinationNode.Id}";
                
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
        
        protected VisitorNode CreateNode (SyntaxTreeNode syntaxTreeNode)
        {
            var node = new VisitorNode(syntaxTreeNode);
            _ids.Push(node.Id);
            _nodes.Add(node);
            return node;
        }

        protected VisitorLink CreateLink (VisitorNode sourceNode, VisitorNode destinationNode, string label = null)
        {
            var link = new VisitorLink(sourceNode, destinationNode, label);
            _links.Add(link);
            return link;
        }
        
        protected VisitorNode FindNodeById(string id)
        {
            return _nodes.First(node => node.Id == id);
        }

        public Stack<string> _ids;
        public List<VisitorNode> _nodes;
        public List<VisitorLink> _links;
    }
}