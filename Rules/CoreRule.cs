using BFParser.Rules.Combinators;
using Microsoft.VisualBasic.CompilerServices;

namespace BFParser.Rules
{
    public abstract class CoreRule
    {
        public abstract SyntaxTreeNode Parse(string text);
        public abstract Grammar Grammar { get; protected set; }
        public abstract void InitGrammar(Grammar grammar);

        public static CoreRule operator +(CoreRule firstRule, CoreRule secondRule)
        {
            return new RuleConcatenation(firstRule, secondRule);
        }
        
        public static CoreRule operator |(CoreRule firstRule, CoreRule secondRule)
        {
            return new RuleAlternative(firstRule, secondRule);
        }
    }

    public static class R
    {
        public static CoreRule T(string token)
        {
            return new RuleToken(token);
        }
        
        public static CoreRule C(string name)
        {
            return new RuleCallGrammarRule(name);
        }

        public static CoreRule S(CoreRule rule, int minTimes, int maxTimes)
        {
            return new RuleSerial(rule, minTimes, maxTimes);
        }
        
        public static CoreRule ZI(CoreRule rule)
        {
            return new RuleSerial(rule, 0, int.MaxValue);
        }
        
        public static CoreRule OI(CoreRule rule)
        {
            return new RuleSerial(rule, 1, int.MaxValue);
        }
        
        /// <summary>
        /// This function does the same functional as RuleOptional class.
        /// One tries parse rule 0 or 1 times and returns SyntaxTreeNode anyway.
        /// But there is no any default value here. (Default is null)
        /// </summary>
        /// <param name="rule">Internal rule for parsing</param>
        /// <returns>SyntaxTreeNode class object</returns>
        public static CoreRule OPT(CoreRule rule)
        {
            return new RuleSerial(rule, 0, 1);
        }
    }
}