using System.Text.RegularExpressions;

namespace BFParser.Rules
{
    public class RuleReg : CoreRule
    {
        public string Name { get; }
        public Regex RExp { get; }

        public RuleReg(string rule)
        {
            Name = $"Rule-[{rule}]";
            RExp = new Regex(@$"^(\s+)*(?<ParsedText>{rule})(?<Rest>.+)*"); // Токен в начале строки, остаток в отдельной группе 
        }

        public override SyntaxTreeNode Parse(string text)
        {
            var pResult = RExp.Match(text);
            var token = pResult.Groups["ParsedText"].Value;
            var rest = pResult.Groups["Rest"].Value;
            return new SyntaxTreeNode(token, rest, null);
        }
    }
}