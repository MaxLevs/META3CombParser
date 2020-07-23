using System;
using System.Text.RegularExpressions;
using BFParser.Rules.DebugTools;

namespace BFParser.Rules
{
    public class RuleToken : CoreRule
    {
        public string Token { get; }

        public RuleToken(string token)
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
            return new SyntaxTreeNode(Token, rest, GrammarRootRuleName, null);
        }

        public override string GrammarRootRuleName { get; protected set; }
        public override Grammar Grammar { get; protected set; }
        public override void InitGrammar(Grammar grammar, string grammarRootRuleName)
        {
            Grammar = grammar;
            GrammarRootRuleName = grammarRootRuleName;
        }
        
        public override void Visit(CoreRuleVisitor visitor)
        {
            visitor.Apply(this);
        }

        public override string ToString()
        {
            return "RuleToken{" + Token + "}";
        }
    }
}