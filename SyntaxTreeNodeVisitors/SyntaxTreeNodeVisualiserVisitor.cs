using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BFParser.Rules.DebugTools;
using GiGraph.Dot.Entities.Attributes.Enums;
using GiGraph.Dot.Entities.Edges;
using GiGraph.Dot.Entities.Graphs;
using GiGraph.Dot.Entities.Nodes;
using GiGraph.Dot.Entities.Types.Styles;
using GiGraph.Dot.Extensions;

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
            var graph = new DotGraph(directed:true);
            graph.Attributes.Label = "Abstract Syntax Tree";
            graph.Attributes.EdgeShape = DotEdgeShape.Orthogonal;
            graph.Nodes.AddRange(_nodes.Select(vnode =>
            {
                var dotNode = new DotNode(vnode.Id);
                dotNode.Attributes.Label = "SyntaxTreeNode["+ vnode.Node.Rule +"]{ " + ( vnode.Node.ParsedText is null || vnode.Node.ParsedText == string.Empty ? "[[NULL]]" : vnode.Node.ParsedText ) + " }{ "+ ( vnode.Node.Rest is null || vnode.Node.Rest == string.Empty ? "[[NULL]]" : vnode.Node.Rest ) +" }";
                dotNode.Attributes.Shape = DotNodeShape.Box;
                dotNode.Attributes.Style.FillStyle = DotNodeFillStyle.Normal;
                return dotNode;
            }));
            graph.Edges.AddRange(_links.Select(link => {
                var edge = new DotEdge(link.SourceNode.Id, link.DestinationNode.Id);
                edge.Attributes.Label = link.Label ?? "";
                return edge;
            }));
            
            return graph.Build();
            
            string result = "digraph {\n";
            result += "graph [label=\"abstract syntax tree\", splines=ortho, nodesep=1.2];\n";
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

                    result += $"label=\"{Label ?? ""}\",";
                    
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