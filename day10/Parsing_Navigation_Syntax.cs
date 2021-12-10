using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using common;
using NUnit.Framework;

namespace day10
{
    [TestFixture]
    public class Parsing_Navigation_Syntax
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase("day10.sample.txt", 26397)]
        [TestCase("day10.input.txt", 0)]
        public void Finds_Invalid_Closing_Brackets(string resource, int expectedScore)
        {
            var lines = Resources.GetResourceLines(typeof(Parsing_Navigation_Syntax), resource);
            var exceptions = new List<ParsingException>();

            foreach (var line in lines)
            {
                Console.Write(line);

                var tokenizer = new NavTokenizer();
                var tokens = tokenizer.Tokenize(line);
                var parser = new NavParser();
                try
                {
                    parser.Parse(tokens);
                    Console.WriteLine(" - Inconclusive");
                }
                catch (ParsingException ex)
                {
                    exceptions.Add(ex);
                    Console.WriteLine(" - " + ex.Message);
                }
            }

            var pointsTable = new Dictionary<NavToken, int>
            {
                { NavTokens.RParen, 3 },
                { NavTokens.RSquare, 57 },
                { NavTokens.RCurl, 1197 },
                { NavTokens.GT, 25137 },
            };

            var score = exceptions.Sum(x => pointsTable[x.Token]);

            Assert.AreEqual(expectedScore, score);
        }

        [Test]
        [TestCaseSource("SimpleBracketsCases")]
        public void Tokenizes_Simple_Brackets(string input, NavToken[] expected)
        {
            var tokenizer = new NavTokenizer();
            var tokens = tokenizer.Tokenize(input);
            Assert.That(tokens, Is.EquivalentTo(expected));
        }

        [Test]
        [TestCase("()")]
        [TestCase("[]")]
        public void Parses_AST_Of_Empty_Groups(string input)
        {
            var tokenizer = new NavTokenizer();
            var tokens = tokenizer.Tokenize(input);
            var parser = new NavParser();
            var ast = parser.Parse(tokens);

            Assert.That(ast.Nodes.Single(), Is.InstanceOf<NodeGroup>());
        }

        public static IEnumerable<object[]> SimpleBracketsCases()
        {
            yield return new object[] { "()", new[] { NavTokens.LParen, NavTokens.RParen } };
            yield return new object[] { "[]", new[] { NavTokens.LSquare, NavTokens.RSquare } };
            yield return new object[] { "{}", new[] { NavTokens.LCurl, NavTokens.RCurl } };
            yield return new object[] { "<>", new[] { NavTokens.LT, NavTokens.GT } };
        }
    }

    public class NavParser
    {
        private int index;
        private Ast result;
        private Node curNode;
        private NavToken curToken;

        public Ast Parse(NavToken[] tokens)
        {
            index = 0;
            result = new Ast();
            curNode = result;

            while (index < tokens.Length)
            {
                curToken = tokens[index];

                Action action = curToken switch
                {
                    { Type: TokenType.GroupStart } => StartGroup,
                    { Type: TokenType.GroupEnd } => EndGroup,
                    _ => throw new Exception("Unknown token type")
                };

                action();

                index++;
            }

            return result;
        }

        private void StartGroup()
        {
            var groupNode = new NodeGroup(curNode, curToken);
            curNode.Add(groupNode);
            curNode = groupNode;
        }

        private void EndGroup()
        {
            Dictionary<NavToken, NavToken> expectedStarts = new Dictionary<NavToken, NavToken>
            {
                {NavTokens.RParen, NavTokens.LParen},
                {NavTokens.RCurl, NavTokens.LCurl},
                {NavTokens.RSquare, NavTokens.LSquare},
                {NavTokens.GT, NavTokens.LT},
            };

            Dictionary<NavToken, NavToken> expectedEnds = new Dictionary<NavToken, NavToken>
            {
                {NavTokens.LParen, NavTokens.RParen},
                {NavTokens.LCurl, NavTokens.RCurl},
                {NavTokens.LSquare, NavTokens.RSquare},
                {NavTokens.LT, NavTokens.GT},
            };

            var curGroup = curNode as NodeGroup;
            if (curGroup == null) throw new Exception("No group to close");

            var expectedStartToken = expectedStarts[curToken];
            if (!curGroup.StartToken.Equals(expectedStartToken))
            {
                throw new GroupCloseException(curToken, index, expectedEnds[curGroup.StartToken]);
            }

            if (curNode.Parent == null)
            {
                throw new Exception("Unexpected token. Expected EOF.");
            }

            curNode = curNode.Parent;
        }
    }

    public class ParsingException : Exception
    {
        public NavToken Token { get; }
        public int Column { get; }

        public ParsingException(NavToken token, int column, string message)
            : base(message)
        {
            Token = token;
            Column = column;
        }
    }

    public class GroupCloseException : ParsingException
    {
        public NavToken Expected { get; }

        public GroupCloseException(NavToken token, int column, NavToken expected)
            : base(token, column, $"Col: {column}: Expected '{expected}, but found '{token}' instead.")
        {
            Expected = expected;
        }
    }

    public class Ast : Node
    {
        private List<Node> nodes;

        public Ast()
            : base(null)
        {
            Nodes = nodes = new List<Node>();
        }

        public override void Add(Node child)
        {
            nodes.Add(child);
        }
    }

    public abstract class Node
    {
        public Node Parent { get; }
        public IEnumerable<Node> Nodes { get; protected set; }

        public Node(Node parent)
        {
            Parent = parent;
            Nodes = Array.Empty<Node>();
        }

        public virtual void Add(Node child)
        {
            throw new NotImplementedException();
        }
    }

    public class NodeGroup : Node
    {
        private readonly NavToken startToken;
        private readonly List<Node> nodes;

        public NavToken StartToken => startToken;

        public NodeGroup(Node parent, NavToken startToken) : base(parent)
        {
            this.startToken = startToken;
            Nodes = nodes = new List<Node>();
        }

        public override void Add(Node child)
        {
            nodes.Add(child);
        }
    }

    public readonly struct NavToken
    {
        public readonly char Character;
        public readonly TokenType Type;

        public NavToken(char character, TokenType type)
        {
            Character = character;
            Type = type;
        }

        public override string ToString()
        {
            return new(Character, 1);
        }
    }

    public enum TokenType
    {
        GroupStart,
        GroupEnd
    };

    public class NavTokens
    {
        public static readonly NavToken LParen = new('(', TokenType.GroupStart);
        public static readonly NavToken RParen = new(')', TokenType.GroupEnd);
        public static readonly NavToken LSquare = new('[', TokenType.GroupStart);
        public static readonly NavToken RSquare = new(']', TokenType.GroupEnd);
        public static readonly NavToken LCurl = new('{', TokenType.GroupStart);
        public static readonly NavToken RCurl = new('}', TokenType.GroupEnd);
        public static readonly NavToken LT = new('<', TokenType.GroupStart);
        public static readonly NavToken GT = new('>', TokenType.GroupEnd);

        private static readonly Dictionary<char, NavToken> all;
        public static bool IsValid(char character) => all.ContainsKey(character);
        public static NavToken Get(char character) => IsValid(character) ? all[character] : throw new Exception($"Unknown token '{character}'");

        static NavTokens()
        {
            all = typeof(NavTokens)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(x => x.FieldType == typeof(NavToken))
                .Select(f => (NavToken)f.GetValue(null)!)
                .ToDictionary(x => x.Character);
        }
    }

    public class NavTokenizer
    {
        public NavToken[] Tokenize(string input)
        {
            List<NavToken> tokens = new List<NavToken>();
            for (var i = 0; i < input.Length; i++)
            {
                tokens.Add(NavTokens.Get(input[i]));
            }
            return tokens.ToArray();
        }
    }
}