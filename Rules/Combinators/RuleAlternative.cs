namespace BFParser.Rules.Combinators
{
    public class RuleAlternative : CoreRule
    {
        public CoreRule FirstRule { get; }
        public CoreRule SecondRule { get; }

        public RuleAlternative(CoreRule firstRule, CoreRule secondRule)
        {
            FirstRule = firstRule;
            SecondRule = secondRule;
        }

        public override SyntaxTreeNode Parse(string text)
        {
            var pResult = FirstRule.Parse(text) ?? SecondRule.Parse(text);
            return pResult;
        }
    }
}