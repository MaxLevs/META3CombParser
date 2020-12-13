using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BFParser.Parsers;
using BFParser.SyntaxTreeNodeVisitors;

namespace BFParser
{
    public class SyntaxTreeNode
    {
        public Guid Id { get; }
        public string ParsedText { get; }
        public string Rest { get; }
        public string RuleName => Parser.RuleName;
        public CoreParser Parser { get; }
        public ReadOnlyCollection<SyntaxTreeNode> Children { get; }

        public SyntaxTreeNode(string parsedText, string rest, CoreParser parserWhereWasProducted, IList<SyntaxTreeNode> children)
        {
            Id = Guid.NewGuid();
            ParsedText = parsedText;
            Rest = rest;
            Parser = parserWhereWasProducted;
            Children = (children is null) ? null : new ReadOnlyCollection<SyntaxTreeNode>(children);
        }

        public void Apply(CoreSyntaxTreeNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        
        /// <summary>
        /// Get visualised AST
        /// </summary>
        /// <returns>String with graph in dot format</returns>
        public string Dot()
        {
            var visitor = new SyntaxTreeNodeVisualiserVisitor();
            Apply(visitor);
            return visitor.GetResult() as string;
        }

        /// <summary>
        /// It helps get clear abstract syntax tree
        /// </summary>
        /// <returns>Returns SyntaxTreeNode graph without trash nodes</returns>
        public SyntaxTreeNode Clear()
        {
            var visitor = new SyntaxTreeNodeClearVisitor();
            Apply(visitor);
            return visitor.GetResult() as SyntaxTreeNode;
        }
    }
}