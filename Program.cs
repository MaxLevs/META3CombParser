using System;
using BFParser.Rules;
using BFParser.Rules.Combinators;

namespace BFParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Grammar gramm = new Grammar
            {
                {"basicOperations", R.T("+") | R.T("-") | R.T(">") | R.T("<")},
                {"loop", R.T("[") + R.C("sourceItem") + R.T("]")},
                {"sourceItem", R.S(R.C("basicOperations") | R.C("loop"), 0, int.MaxValue)}
            };
            gramm.InitGrammar();
            
            var node = gramm["sourceItem"].Parse("++++++[->+++>+<<]");
            
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