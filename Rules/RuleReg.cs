using System.Text.RegularExpressions;
using BFParser.Rules.DebugTools;

namespace BFParser.Rules
{
    public class RuleReg : CoreRule
    {
        public string ExprBase { get; }
        public Regex RExp { get; }

        public RuleReg(string rule)
        {
            ExprBase = $"Rule-[{rule}]";
            RExp = new Regex(@$"^(\s+)*(?<ParsedText>{rule})(?<Rest>.+)*"); // Токен в начале строки, остаток в отдельной группе 
        }

        public override SyntaxTreeNode Parse(string text)
        {
            var pResult = RExp.Match(text);
            var token = pResult.Groups["ParsedText"].Value;
            var rest = pResult.Groups["Rest"].Value;
            return pResult.Success ? new SyntaxTreeNode(token, rest, this, null) : null;
        }

        public override Grammar Grammar { get; protected set; }
        public override void InitGrammar(Grammar grammar)
        {
            Grammar = grammar;
        }
        
        public override void Visit(CoreRuleVisitor visitor)
        {
            visitor.Apply(this);
        }
        
        
        public override string ToString()
        {
            var fixStringRegex = new Regex("\"");
            var newFixedExprBase = fixStringRegex.Replace(ExprBase, "\\\"");
            return "RuleReg{" + newFixedExprBase + "}";
        }
    }
}