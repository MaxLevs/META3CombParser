using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BFParser
{
    public class SyntaxTreeNode
    {
        public Guid Id { get; }
        public string ParsedText { get; }
        public string Rest { get; }
        public ReadOnlyCollection<SyntaxTreeNode> Children { get; }

        public SyntaxTreeNode(string parsedText, string rest, IList<SyntaxTreeNode> children)
        {
            Id = Guid.NewGuid();
            ParsedText = parsedText;
            Rest = rest;
            Children = (children is null) ? null : new ReadOnlyCollection<SyntaxTreeNode>(children);
        }
    }
}