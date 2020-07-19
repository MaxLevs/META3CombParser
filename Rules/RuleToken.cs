using System.Text.RegularExpressions;

namespace BFParser.Rules
{
    public class RuleToken : CoreRule
    {
        public string Name { get; }
        public Regex RExp { get; }

        public RuleToken(string token)
        {
            Name = token;
            RExp = new Regex(@$"^(\s+)*(?<Token>{token})(?<Rest>.+)*"); // Токен в начале строки, остаток в отдельной группе 
        }

        public override SyntaxTreeNode Parse(string text)
        {
            var pResult = RExp.Match(text);
            var token = pResult.Groups["Token"].Value;
            var rest = pResult.Groups["Rest"].Value;
            return (pResult.Success) ? new SyntaxTreeNode(token, rest, null) : null;
        }
    }
}