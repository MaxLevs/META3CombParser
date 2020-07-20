using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}