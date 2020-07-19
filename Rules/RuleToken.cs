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
            //TODO: There is a problem: if token is "login" and text is "loginsomethingelse" than result is "login" and rest is "somethingelse"
            RExp = new Regex(@$"^(\s+)*(?<Token>{token})(?<Rest>.+)*"); // Токен в начале строки, остаток в отдельной группе 
        }

        public override SyntaxTreeNode Parse(string text)
        {
            var pResult = RExp.Match(text);
            var token = pResult.Groups["Token"].Value;
            var rest = pResult.Groups["Rest"].Value;
            return (pResult.Success) ? new SyntaxTreeNode(token, rest, null) : null;
        }

        public override Grammar Grammar { get; protected set; }
        public override void InitGrammar(Grammar grammar)
        {
            Grammar = grammar;
        }
    }
}