using System;
using System.Collections.Generic;
using System.Linq;

namespace BFParser.SyntaxTreeNodeVisitors
{
    public class SyntaxTreeNodeClearVisitor : CoreSyntaxTreeNodeVisitor
    {
        public override void Visit(SyntaxTreeNode syntaxTreeNode)
        {
            if (syntaxTreeNode.Children is null || syntaxTreeNode.Children.Count == 0)
            {
                // if (syntaxTreeNode.ParsedText is null)
                //     return;

                _nodes.Add(syntaxTreeNode);
                _ids.Push(syntaxTreeNode.Id);
            }
            
            else
            {
                foreach (var childNode in syntaxTreeNode.Children)
                {
                    childNode.Apply(this);
                }

                var nodes = new List<SyntaxTreeNode>();
                for(int i = 0; i < syntaxTreeNode.Children.Count; ++i)
                {
                    var id = _ids.Pop();
                    var node = _nodes.Find(element => element.Id == id);
                    if (node?.ParsedText != null)
                    {
                        nodes.Add(node);
                    }
                }

                nodes.Reverse();

                switch (nodes.Count)
                {
                    case 0:
                        // Just skip this node
                        return;
                    case 1:
                        // Elevate child node up and skip this node
                        _ids.Push(nodes[0].Id);
                        break;
                    default:
                    {
                        var changedNode = new SyntaxTreeNode(syntaxTreeNode.ParsedText, syntaxTreeNode.Rest, null, nodes);
                        _nodes.Add(changedNode);
                        _ids.Push(changedNode.Id);
                        break;
                    }
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