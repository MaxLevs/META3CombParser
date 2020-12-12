using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BFParser.DebugTools;
using BFParser.Parsers;
using BFParser.Parsers.Combinators;
using BFParser.Parsers.DebugTools;

namespace BFParser
{
    public class Grammar : IDictionary<string, CoreParser>
    {
        private readonly Dictionary<string, CoreParser> _rules;
        private readonly string _goalRuleName;

        public Grammar(string goalRuleName, IDictionary<string, CoreParser> old = null)
        {
            _goalRuleName = goalRuleName;
            _rules = old is null ? new Dictionary<string, CoreParser>() : new Dictionary<string, CoreParser>(old);
        }

        public IEnumerator<KeyValuePair<string, CoreParser>> GetEnumerator()
        {
            return _rules.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<string, CoreParser> item)
        {
            _rules.Add(item.Key, item.Value);
        }

        public void Add(string key, CoreParser value)
        {
            _rules.Add(key, value);
        }

        public void Clear()
        {
            _rules.Clear();
        }

        public bool Contains(KeyValuePair<string, CoreParser> item)
        {
            return _rules.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, CoreParser>[] array, int arrayIndex)
        {
            var result = new KeyValuePair<string,CoreParser>[_rules.Count];
            var i = 0;
            foreach (var item in _rules)
            {
                result[i] = item;
                ++i;
            }
        }

        public bool Remove(KeyValuePair<string, CoreParser> item)
        {
            return _rules.Remove(item.Key);
        }

        public int Count => _rules.Count;
        public bool IsReadOnly => false;

        public bool ContainsKey(string key)
        {
            return _rules.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return _rules.Remove(key);
        }

        public bool TryGetValue(string key, out CoreParser value)
        {
            return _rules.TryGetValue(key, out value);
        }

        public CoreParser this[string key]
        {
            get => _rules[key];
            set => _rules[key] = value;
        }

        public ICollection<string> Keys => _rules.Keys;
        public ICollection<CoreParser> Values => _rules.Values;
        public CoreParser Goal => this[_goalRuleName];

        public void InitGrammar()
        {
            foreach (var rule in _rules)
            {
                rule.Value.InitGrammar(this, rule.Key);
            }
        }

        public void Visit(CoreGrammarVisitor<ConvertToDOTVisitor> visitor)
        {
            visitor.Apply(this);
        }

        public string Dot(string startRuleName = null)
        {
            var visitor = new CoreGrammarVisitor<ConvertToDOTVisitor>();
            Visit(visitor);
            return visitor.GetResult(startRuleName ?? _goalRuleName) as string;
        }

        public CoreParser ExpandThis(ParserCallGrammarParser basicParser, List<KeyValuePair<string, List<string>>> operations)
        {
            for (int i = 0; i < operations.Count; ++i)
            {
                var newRuleName = operations[i].Key;
                var ops = operations[i].Value.Select(P.T).Aggregate((current, operation) => current | operation);
                var newRule = basicParser + P.ZI(ops + basicParser);
                Add(newRuleName, newRule);
                basicParser = P.C(newRuleName) as ParserCallGrammarParser;
            }

            return basicParser;
        }
    }
}