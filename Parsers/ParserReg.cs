using System.Text.RegularExpressions;
using BFParser.Parsers.DebugTools;

namespace BFParser.Parsers
{
    public class ParserReg : CoreParser
    {
        public string ExprBase { get; }
        public Regex RExp { get; }

        public ParserReg(string rule)
        {
            ExprBase = $"Rule-[{rule}]";
            // [todo] change this
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
            var fixStringRegex = new Regex("\"");
            var newFixedExprBase = fixStringRegex.Replace(ExprBase, @"\""");
            return "RuleReg{" + newFixedExprBase + "}";
        }
    }
}