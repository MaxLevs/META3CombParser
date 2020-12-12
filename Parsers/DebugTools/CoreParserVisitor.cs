using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using BFParser.Parsers.Combinators;

namespace BFParser.Parsers.DebugTools
{
    public abstract class CoreParserVisitor
    {
        #region Apply Methods
        public abstract void Apply(ParserToken parser);
        public abstract void Apply(ParserReg parser);
        public abstract void Apply(ParserConcatenation parser);
        public abstract void Apply(ParserAlternative parser);
        public abstract void Apply(ParserOptional parser);
        public abstract void Apply(ParserSerial parser);
        public abstract void Apply(ParserCallGrammarParser parser);
        #endregion
        
        #region Subclasses
        public class VisitorNode
        {
            public Guid Id { get; }
            public string Token { get; }
            
            public VisitorNodeType Type { get; }

            public VisitorNode(string token, VisitorNodeType type)
            {
                Id = Guid.NewGuid();
                Token = token;
                this.Type = type;
            }

            public override string ToString()
            {
                var sId = "n" + Regex.Replace(Id.ToString(), "-", "");
                var shapes = new Dictionary<VisitorNodeType, string>
                {
                    {VisitorNodeType.Termimal, "oval"},
                    {VisitorNodeType.Combinator, "box"},
                    {VisitorNodeType.Call, "doubleoctagon"},
                    {VisitorNodeType.Start, "Mdiamond"},
                };
                var styles = new Dictionary<VisitorNodeType, string>
                {
                    {VisitorNodeType.Termimal, "default"},
                    {VisitorNodeType.Combinator, "filled"},
                    {VisitorNodeType.Call, "filled"},
                    {VisitorNodeType.Start, "defult"}
                };
                return $"{sId} [label=\"{Token}\", shape={shapes[Type]}, style={styles[Type]}];";
            }

            public enum VisitorNodeType { Termimal, Combinator, Call, Start }
            
        }

        public class VisitorLink
        {
            public Guid Id { get; }
            public string Label { get; private set; }
            public VisitorNode SourceNode { get; }
            public VisitorNode DestinationNode { get; }
            public string Style { get; private set; }

            public VisitorLink(VisitorNode sourceNode, VisitorNode destinationNode)
            {
                Id = Guid.NewGuid();
                SourceNode = sourceNode;
                DestinationNode = destinationNode;
            }

            public override string ToString()
            {
                var sId = "n" + Regex.Replace(SourceNode.Id.ToString(), "-", "");
                var dId = "n" + Regex.Replace(DestinationNode.Id.ToString(), "-", "");
                
                var result = $"{sId} -> {dId}";
                if (!(Label is null && Style is null))
                {
                    result += " [";

                    result += Label is null ? "" : $"label=\"{Label}\",";
                    result += Style is null ? "" : $"style=\"{Style}\",";
                    
                    result += "]";
                }
                result += ";";
                
                return result;
            }


            public VisitorLink SetLabel(string label)
            {
                Label = label;
                return this;
            }
            
            public VisitorLink SetStyle(string style)
            {
                Style = style;
                return this;
            }
        }
        
        public class VisitorCall
        {
            public Guid Id { get; }
            public VisitorNode SourceNode { get; }
            public string RuleName { get; }

            public VisitorCall(VisitorNode sourceNode, string ruleName)
            {
                Id = Guid.NewGuid();
                SourceNode = sourceNode;
                RuleName = ruleName;
            }
        }
        #endregion
        
        #region HelpMethods
        protected VisitorNode CreateNode (string token, VisitorNode.VisitorNodeType nodeType = VisitorNode.VisitorNodeType.Termimal)
        {
            var node = new VisitorNode(token, nodeType);
            _ids.Push(node.Id);
            _nodes.Add(node);
            return node;
        }

        protected VisitorLink CreateLink (VisitorNode sourceNode, VisitorNode destinationNode, string label = null)
        {
            var link = new VisitorLink(sourceNode, destinationNode);
            if (label != null) 
                link.SetLabel(label);
            _links.Add(link);
            return link;
        }
        
        protected VisitorCall CreateCall (VisitorNode callNode, string ruleName)
        {
            var call = new VisitorCall(callNode, ruleName);
            _calls.Add(call);
            return call;
        }

        protected VisitorNode FindNodeById(Guid id)
        {
            return _nodes.First(node => node.Id == id);
        }
        #endregion
        
        protected Stack<Guid> _ids;
        protected List<VisitorNode> _nodes;
        protected List<VisitorLink> _links;
        protected List<VisitorCall> _calls;

        public abstract object GetResult(string name);
        
        public ReadOnlyCollection<VisitorCall> Calls => new ReadOnlyCollection<VisitorCall>(_calls);
        public VisitorNode Root => _nodes[^1];
    }
}