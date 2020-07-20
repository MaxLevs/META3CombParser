using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BFParser.Rules.DebugTools;

namespace BFParser.SyntaxTreeNodeVisitors
{
    public class SyntaxTreeNodeVisualiserVisitor : CoreSyntaxTreeNodeVisitor
    {
        public override void Apply(SyntaxTreeNode syntaxTreeNode)
        {
            if (syntaxTreeNode.Children is null)
            {
                CreateNode(syntaxTreeNode);
                return;
            }
            
            var childrenNodes = new List<VisitorNode>();
            foreach (var child in syntaxTreeNode.Children)
            {
                child.Visit(this);
                childrenNodes.Add(FindNodeById(_ids.Pop()));
            }
            var parentNode = CreateNode(syntaxTreeNode);
            for (var i = 0; i < childrenNodes.Count; ++i)
            {
                CreateLink(parentNode, childrenNodes[i], (i+1).ToString());
            }
        }

        public override object GetResult()
        {
            string result = "digraph {\n";
            result += "graph [label=\"Abstract Syntax Tree\", splines=ortho, nodesep=1.2];\n";
            result += "node[shape=box,style=filled];\n\n";
            result = _nodes.Aggregate(result, (current, node) => current += node.ToString() + "\n") + "\n\n";
            result = _links.Aggregate(result, (current, link) => current += link.ToString() + "\n");
            result += "}\n";

            return result;
        }

        public SyntaxTreeNodeVisualiserVisitor()
        {
            _ids = new Stack<string>();
            _nodes = new List<VisitorNode>();
            _links = new List<VisitorLink>();
        }
        
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