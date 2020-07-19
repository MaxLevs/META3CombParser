using System;
using BFParser.Rules;
using BFParser.Rules.Combinators;

namespace BFParser
{
    class Program
    {
        static void Main(string[] args)
        {
            CoreRule ruleLogin = new RuleToken("login");
            CoreRule ruleMaxlevs = new RuleToken("maxlevs");
            CoreRule rulePasswd = new RuleToken("passwd");
            CoreRule ruleKektus = new RuleToken("kektus");
            
            CoreRule rule = new RuleConcatenation(ruleLogin, ruleMaxlevs);
            var node = rule.Parse(" login maxlevs passwd kektus");
            
            PrintR(node);
        }

        static void PrintR(SyntaxTreeNode node)
        {
            if (node is null)
            {
                Console.WriteLine("[NULL]");
                return;
            }
            
            Console.WriteLine(node.Id);
            Console.WriteLine(node.ParsedText);
            Console.WriteLine(node.Rest);
            Console.WriteLine(node.Children);
        }
    }
}