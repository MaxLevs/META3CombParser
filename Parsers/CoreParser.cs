using BFParser.Parsers.Combinators;
using BFParser.Parsers.DebugTools;
using Microsoft.VisualBasic.CompilerServices;

namespace BFParser.Parsers
{
    public abstract class CoreParser
    {
        public abstract SyntaxTreeNode Parse(string text);
        public abstract Grammar Grammar { get; protected set; }
        public abstract void InitGrammar(Grammar grammar);

        public static CoreParser operator +(CoreParser firstParser, CoreParser secondParser)
        {
            return new ParserConcatenation(firstParser, secondParser);
        }
        
        public static CoreParser operator |(CoreParser firstParser, CoreParser secondParser)
        {
            return new ParserAlternative(firstParser, secondParser);
        }

        public abstract void Visit(CoreParserVisitor visitor);
    }

    public static class P
    {
        public static CoreParser T(string token)
        {
            return new ParserToken(token);
        }

        public static CoreParser RE(string rexpr)
        {
            return new ParserReg(rexpr);
        }
        
        public static CoreParser C(string name)
        {
            return new ParserCallGrammarParser(name);
        }

        public static CoreParser S(CoreParser parser, int minTimes, int maxTimes)
        {
            return new ParserSerial(parser, minTimes, maxTimes);
        }
        
        public static CoreParser ZI(CoreParser parser)
        {
            return new ParserSerial(parser, 0, int.MaxValue);
        }
        
        public static CoreParser OI(CoreParser parser)
        {
            return new ParserSerial(parser, 1, int.MaxValue);
        }
        
        /// <summary>
        /// This function does the same functional as RuleOptional class.
        /// One tries parse rule 0 or 1 times and returns SyntaxTreeNode anyway.
        /// But there is no any default value here. (Default is null)
        /// </summary>
        /// <param name="parser">Internal rule for parsing</param>
        /// <returns>SyntaxTreeNode class object</returns>
        public static CoreParser OPT(CoreParser parser)
        {
            return new ParserSerial(parser, 0, 1);
        }

        public static CoreParser MB(CoreParser parser)
        {
            return new ParserOptional(parser);
        }

        public static CoreParser SEQ(CoreParser parser, string delimiter)
        {
            return MB(parser + T(delimiter) | parser);
        }
    }
}