using System;
using System.Text.RegularExpressions;
using BFParser.Parsers.DebugTools;

namespace BFParser.Parsers
{
    public class ParserToken : CoreParser
    {
        public string Token { get; }

        public ParserToken(string token)
        {
            Token = token;
        }

        public override SyntaxTreeNode Parse(string text)
        {
            text = text.Trim(' ', '\t', '\n', '\v', '\f', '\r');
            var res = text.IndexOf(Token, StringComparison.Ordinal);
            if (res != 0)
            {
                return null;
            }
            var rest = text.Substring(Token.Length, text.Length - Token.Length);
            return new SyntaxTreeNode(Token, rest, this, null);
        }

        public override Grammar Grammar { get; protected set; }
        public override void InitGrammar(Grammar grammar, string ruleName)
        {
            Grammar = grammar;
            RuleName = ruleName;
        }
        
        public override void Visit(CoreParserVisitor visitor)
        {
            visitor.Apply(this);
        }

        public override string ToString()
        {
            return "RuleToken{" + Token + "}";
        }
    }
}