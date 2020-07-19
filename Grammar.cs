using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BFParser.Rules;

namespace BFParser
{
    public class Grammar : IDictionary<string, CoreRule>
    {
        private readonly Dictionary<string, CoreRule> _rules;

        public Grammar(IDictionary<string, CoreRule> old = null)
        {
            if (old is null)
            {
                _rules = new Dictionary<string, CoreRule>();
            }

            else
            {
                _rules = new Dictionary<string, CoreRule>(old);
            }
        }

        public IEnumerator<KeyValuePair<string, CoreRule>> GetEnumerator()
        {
            return _rules.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<string, CoreRule> item)
        {
            _rules.Add(item.Key, item.Value);
        }

        public void Add(string key, CoreRule value)
        {
            _rules.Add(key, value);
        }

        public void Clear()
        {
            _rules.Clear();
        }

        public bool Contains(KeyValuePair<string, CoreRule> item)
        {
            return _rules.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, CoreRule>[] array, int arrayIndex)
        {
            var result = new KeyValuePair<string,CoreRule>[_rules.Count];
            var i = 0;
            foreach (var item in _rules)
            {
                result[i] = item;
                ++i;
            }
        }

        public bool Remove(KeyValuePair<string, CoreRule> item)
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

        public bool TryGetValue(string key, out CoreRule value)
        {
            return _rules.TryGetValue(key, out value);
        }

        public CoreRule this[string key]
        {
            get => _rules[key];
            set => _rules[key] = value;
        }

        public ICollection<string> Keys => _rules.Keys;
        public ICollection<CoreRule> Values => _rules.Values;

        public void InitGrammar()
        {
            foreach (var rule in _rules)
            {
                rule.Value.InitGrammar(this);
            }
        }
    }
}