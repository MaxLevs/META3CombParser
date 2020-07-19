using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BFParser
{
    public class SyntaxTreeNode
    {
        public int Id { get; }
        public string Token { get; }
        public ReadOnlyCollection<SyntaxTreeNode> Children { get; }

        public SyntaxTreeNode(int id, string token, IList<SyntaxTreeNode> children)
        {
            Id = id;
            Token = token;
            Children = new ReadOnlyCollection<SyntaxTreeNode>(children);
        }
    }
}