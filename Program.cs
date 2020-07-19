using System;
using BFParser.DebugTools;
using BFParser.Rules;
using BFParser.Rules.Combinators;
using BFParser.Rules.DebugTools;

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
                {"sourceItem", R.ZI(R.C("basicOperations") | R.C("loop"))}
            };
            gramm.InitGrammar();
            
            var visitor = new CoreGrammarVisitor<ConvertToDOTVisitor>();
            gramm.Visit(visitor);

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