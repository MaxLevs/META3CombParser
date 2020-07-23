# META3CombParser
Just parser lib for my projects

## How to specificate a grammar
For using this lib you must clone this repo as submodule and create subclass what is named as ConcreteGrammar.cs with something like that
```cs
using System.Collections.Generic;
using BFParser;
using BFParser.Rules;
using BFParser.Rules.Combinators;

namespace BFCompiler
{
    public static class ConcreteGrammar
    {
        public static Grammar GetGrammar()
        {
            return GetBrainFuck();
        }

        private static Grammar GetBrainFuck()
        {
            var gram = new Grammar("sourceItem")
            {
                {"basicOperations", R.T("+") | R.T("-") | 
                                    R.T("<") | R.T(">") | 
                                    R.T(".") | R.T(",")},
                {"loop", R.T("[") + R.C("sourceItem") + R.T("]")},
                {"sourceItem", R.ZI(R.C("basicOperations") | R.C("loop"))}
            };
            gram.InitGrammar();
            
            return gram;
        }
    }
}
```

You must specify name of main rule in Grammar class counstructor.
Grammar is instance of IDictionary. 
Keys is rule names. One of this must be specifyed as main rule in constuctor.
Values of Grammar is instatnce of rule classes.

There are static shortcuts for writing rules in R class.

Also you can use Grammar.ExpandThis() method for building expressions with priority step by step.
First argument is "term"-rule for start building. Second argument is list of pairs (name for rule , list with operations). For example
```cs
private static Grammar GetMathExpr()
{
    var gram = new Grammar("expr")
    {
        {"value", R.RE(@"[0-9]+") | R.C("grouping")},
        {"grouping", R.T("(") + R.C("sum") + R.T(")")},
    };
    var exprRuleCall = gram.ExpandThis(R.C("value") as RuleCallGrammarRule, new List<KeyValuePair<string, List<string>>>
    {
        new KeyValuePair<string, List<string>>("product", new List<string>{"*", "/", "%"}),
        new KeyValuePair<string, List<string>>("sum", new List<string>{"+", "-"}),
    });
    gram.Add("expr", exprRuleCall);
    gram.InitGrammar();

    return gram;
}
```

## How to use specificated grammar
When you have an object of Grammar class, you can start parsing a text. 
Every rule have `Parse(string text)` method. And the grammar object has Goal property. It's a rule are stecificated by name in grammar constructor.
Just call `Parse(string text)` method of goal rule. It returns an ATS's root node as an object of SyntaxTreeNode class.
You can also call `Dot()` method for getting graphviz essence of the parser and the AST. It returns a text you can use for plotting a graph in graphviz tools.

For example
```cs
var gram = ConcreteGrammar.GetGrammar();
string parserStructure = gram.Dot(); // Let's see what does the parser look like.

var goalRule = gram.Goal;
var astRootNode = goalRule.Parse("<some text here>"); // This is our parsed AST

// In this situation there are some trash nodes. Let's make the AST clear.
astRootNode = astRootNode.Clear();

// Getting dot visualisation of AST
string astVisualisation = astRootNode.Dot();
```
