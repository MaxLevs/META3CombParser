using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BFParser.Parsers.Combinators;

namespace BFParser.Parsers.DebugTools
{
    public class ConvertToDOTVisitor : CoreParserVisitor
    {
        public override object GetResult(string name)
        {
            string essence = "subgraph cluster_" + name + "{\n" +
                             "label=\"" + name + "\";\n" +
                             "color=blue;\n" +
                             "nodesep=1;\n" +
                             "node [style=filled];\n" +
                             "\n";
                
            essence = _nodes.Aggregate(essence, (current, node) => current + ("\t" + node + "\n")) + "\n";
            essence = _links.Aggregate(essence, (current, link) => current + ("\t" + link + "\n"));
            essence += "}\n";
            return essence;
        }
        
        public ConvertToDOTVisitor()
        {
            _nodes = new List<VisitorNode>();
            _links = new List<VisitorLink>();
            _calls = new List<VisitorCall>();
            _ids = new Stack<Guid>();
        }

        #region Applyes
        public override void Apply(ParserToken parser)
        {
            CreateNode(parser.Token);
        }

        public override void Apply(ParserReg parser)
        {
            var fixStringRegex = new Regex("\"");
            var newFixedExprBase = fixStringRegex.Replace(parser.ExprBase, @"\""");
            CreateNode(newFixedExprBase);
        }

        public override void Apply(ParserConcatenation parser)
        {
            parser.FirstParser.Visit(this);
            parser.SecondParser.Visit(this);
            var secondVariantDestinationNode = FindNodeById(_ids.Pop());
            var firstVariantDestinationNode = FindNodeById(_ids.Pop());
            var sourceNode = CreateNode("ConcatenationNode(+)", VisitorNode.VisitorNodeType.Combinator);
            CreateLink(sourceNode, firstVariantDestinationNode, "1");
            CreateLink(sourceNode, secondVariantDestinationNode, "2");
        }

        public override void Apply(ParserAlternative parser)
        {
            parser.FirstParser.Visit(this);
            parser.SecondParser.Visit(this);
            var secondVariantDestinationNode = FindNodeById(_ids.Pop());
            var firstVariantDestinationNode = FindNodeById(_ids.Pop());
            var sourceNode = CreateNode("AlternativeNode(|)", VisitorNode.VisitorNodeType.Combinator);
            CreateLink(sourceNode, firstVariantDestinationNode, "1");
            CreateLink(sourceNode, secondVariantDestinationNode, "2");
        }

        public override void Apply(ParserOptional parser)
        {
            parser.InternalParser.Visit(this);
            var destinationNode = FindNodeById(_ids.Pop());
            var sourceNode = CreateNode("OptionalNode(?)", VisitorNode.VisitorNodeType.Combinator);
            CreateLink(sourceNode, destinationNode);
        }

        public override void Apply(ParserSerial parser)
        {
            parser.InternalParser.Visit(this);
            var destinationNode = FindNodeById(_ids.Pop());
            var token =
                $"SerialNode[{parser.MinTimes},{(parser.MaxTimes == int.MaxValue ? "∞" : parser.MaxTimes.ToString())}]";
            var sourceNode = CreateNode(token, VisitorNode.VisitorNodeType.Combinator);
            CreateLink(sourceNode, destinationNode);
        }

        public override void Apply(ParserCallGrammarParser parser)
        {
            var node = CreateNode($"CallNode[{parser.GrammarRuleName}]", VisitorNode.VisitorNodeType.Call);
            CreateCall(node, parser.GrammarRuleName);
        }
        #endregion
    }
}