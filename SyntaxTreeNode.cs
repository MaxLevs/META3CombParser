using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BFParser.Rules;
using BFParser.SyntaxTreeNodeVisitors;

namespace BFParser
{
    public class SyntaxTreeNode
    {
        public Guid Id { get; }
        public string ParsedText { get; }
        public string Rest { get; }
        public CoreRule Rule { get; }
        public ReadOnlyCollection<SyntaxTreeNode> Children { get; }

        public SyntaxTreeNode(string parsedText, string rest, CoreRule ruleWhereWasProducted, IList<SyntaxTreeNode> children)
        {
            Id = Guid.NewGuid();
            ParsedText = parsedText;
            Rest = rest;
            Rule = ruleWhereWasProducted;
            Children = (children is null) ? null : new ReadOnlyCollection<SyntaxTreeNode>(children);
        }

        public void Visit(CoreSyntaxTreeNodeVisitor visitor)
        {
            visitor.Apply(this);
        }

        public string Dot()
        {
            var visitor = new SyntaxTreeNodeVisualiserVisitor();
            Visit(visitor);
            return visitor.GetResult() as string;
        }
    }
}