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
                // if (syntaxTreeNode.ParsedText is null)
                //     return;

                _nodes.Add(syntaxTreeNode);
                _ids.Push(syntaxTreeNode.Id);
            }
            
            else
            {
                foreach (var childNode in syntaxTreeNode.Children)
                {
                    childNode.Visit(this);
                }

                var nodes = new List<SyntaxTreeNode>();
                for(int i = 0; i < syntaxTreeNode.RCount; ++i)
                {
                    var id = _ids.Pop();
                    var node = _nodes.Find(element => element.Id == id);
                    if (node?.ParsedText != null)
                    {
                        nodes.Add(node);
                    }
                }

                nodes.Reverse();

                if (nodes.Count < 2 || nodes.All(node => node.RuleName == syntaxTreeNode.RuleName))
                {
                    foreach (var node in nodes)
                    {
                        // Elevate children node up and skip this node
                        // Or just skip if nodes list is empty
                        _ids.Push(node.Id);
                    }
                }
                else
                {
                    var changedNode = new SyntaxTreeNode(syntaxTreeNode.ParsedText, syntaxTreeNode.Rest, syntaxTreeNode.RuleName, nodes);
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