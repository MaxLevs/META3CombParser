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

            CoreRule rule = ruleLogin + new RuleOptional(ruleMaxlevs) | rulePasswd + ruleKektus ;
            var node = rule.Parse(" login maxlevz passwd kektus");
            
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

            if (node.Children is null) return;
            foreach (var child in node.Children)
            {
                Console.WriteLine();
                PrintR(child);
            }
        }
    }
}