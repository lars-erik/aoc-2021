using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace day16
{
    public class NibbleParser
    {
        const int MoreGroupsMask = 0b10000;

        private int index;
        private string bitString;

        private Dictionary<int, Func<Node>> parsers;
        private NodeId currentId;

        private readonly Dictionary<int, Func<NodeId, Node[], Node>> factories = new()
        {
            { 0, (id, children) => new Sum(id, children) },
            { 1, (id, children) => new Product(id, children) },
            { 2, (id, children) => new Minimum(id, children) },
            { 3, (id, children) => new Maximum(id, children) },
            { 5, (id, children) => new GreaterThan(id, children[0], children[1]) },
            { 6, (id, children) => new LessThan(id, children[0], children[1]) },
            { 7, (id, children) => new Equal(id, children[0], children[1]) },
        };

        public NibbleParser()
        {
            parsers = new Dictionary<int, Func<Node>>
            {
                {4, ParseLiteral },
            };
        }

        public Node Parse(string input)
        {
            bitString = input;
            index = 0;
            
            return ParseNextNode();
        }

        private Node ParseNextNode()
        {
            currentId = new NodeId(NextBits(3), NextBits(3));

            if (parsers.ContainsKey(currentId.TypeId))
                return parsers[currentId.TypeId]();

            return ParseComposite();
        }

        private Node ParseComposite()
        {
            var lengthType = NextBits(1);
            return lengthType == 0
                ? ParseLengthType0()
                : ParseLengthType1();
        }

        private Node ParseLengthType0()
        {
            var length = NextBits(15);
            var children = new List<Node>();
            var id = currentId;
            var start = index;

            while(index < start + length)
            {
                var subNode = ParseNextNode();
                children.Add(subNode);
            }

            var node = factories[id.TypeId](id, children.ToArray());
            return node;
        }

        private Node ParseLengthType1()
        {
            var numberOfChildren = NextBits(11);
            var children = new Node[numberOfChildren];
            var id = currentId;

            for (var i = 0; i < numberOfChildren; i++)
            {
                var subNode = ParseNextNode();
                children[i] = subNode;
            }

            var node = factories[id.TypeId](id, children.ToArray());
            return node;
        }

        private Node ParseLiteral()
        {
            bool hasMoreGroups = true;
            var nibbles = new List<int>();
            while (hasMoreGroups)
            {
                var group = Convert.ToInt32(bitString.Substring(index, 5), 2);
                hasMoreGroups = (@group & MoreGroupsMask) == MoreGroupsMask;
                var nibble = @group & ~MoreGroupsMask;
                nibbles.Add(nibble);
                index += 5;
            }

            var totalBitString = nibbles.Aggregate("", (str, c) => str + NibbleToString(c));
            var total = Convert.ToInt64(totalBitString, 2);
            return new Literal(currentId, total);
        }

        private static string NibbleToString(int c)
        {
            return Convert.ToString(c, 2).PadLeft(4, '0');
        }

        private int NextBits(int length)
        {
            var bits = Convert.ToInt32(bitString.Substring(index, length), 2);
            index += length;
            return bits;
        }

        public static string ConvertToBits(string input)
        {
            var hexNibbles = input.ToCharArray();
            var nibbles = hexNibbles.Select(h => int.Parse(h.ToString(), NumberStyles.AllowHexSpecifier));
            var bitString = String.Join("", nibbles.Select(NibbleToString));
            return bitString;
        }
    }
}