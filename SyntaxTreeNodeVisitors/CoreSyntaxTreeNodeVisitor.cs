using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BFParser.SyntaxTreeNodeVisitors
{
    public abstract class CoreSyntaxTreeNodeVisitor
    {
        public abstract void Visit(SyntaxTreeNode syntaxTreeNode);
        public abstract object GetResult();
    }
}