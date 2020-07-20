using System;
using System.Collections.Generic;
using System.Linq;

namespace BFParser.SyntaxTreeNodeVisitors
{
    public class SyntaxTreeNodeClearVisitor : CoreSyntaxTreeNodeVisitor
    {
        public override void Apply(SyntaxTreeNode syntaxTreeNode)
        {
            if (syntaxTreeNode.Children is null || syntaxTreeNode.Children.Count == 0)
            {
                if (syntaxTreeNode.ParsedText is null)
                    return;

                _nodes.Add(syntaxTreeNode);
                _ids.Push(syntaxTreeNode.Id);
            }
            
            else
            {
                foreach (var childNode in syntaxTreeNode.Children)
                {
                    childNode.Visit(this);
                }

                var nodes = _ids.Select(id => _nodes.Find(node => node.Id == id)).ToList();

                if (nodes.Count == 0)
                {
                    // Just skip this node
                    return;
                }
                else if (nodes.Count == 1)
                {
                    _ids.Push(nodes[0].Id);
                    // Elevate child node up and skip this node
                }
                else
                {
                    var changedNode = new SyntaxTreeNode(syntaxTreeNode.ParsedText, syntaxTreeNode.Rest, null, nodes);
                    _nodes.Add(changedNode);
                    _ids.Push(changedNode.Id);   
                }
            }
        }

        private SyntaxTreeNode _result;
        public override object GetResult()
        {
            if (_result is null)
            {
                var id = _ids.Pop();
                _result = _nodes.Find(node => node.Id == id);
            }

            return _result;
        }

        private Stack<Guid> _ids;
        private List<SyntaxTreeNode> _nodes;
        
        public SyntaxTreeNodeClearVisitor()
        {
            _ids = new Stack<Guid>();
            _nodes = new List<SyntaxTreeNode>();
        }
    }
}