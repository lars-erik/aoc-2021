using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace day16
{
    public class Decoding_Packets
    {
        private NibbleParser parser;

        [SetUp]
        public void Setup()
        {
            parser = new NibbleParser();
        }

        [Test]
        public void Of_TypeID_4_Is_Literal()
        {
            const string input = "D2FE28";
            const int expected = 2021;

            var bitString = ConvertToBits(input);
            Console.WriteLine(bitString);

            var actual = ((Literal)parser.Parse(bitString)).Value;

            Assert.AreEqual(expected, actual);

        }

        [Test]
        [TestCase("38006F45291200", 6, 2, 1)]
        [TestCase("EE00D40C823060", 3, 3, 3)]
        public void Of_TypeIds_Other_Than_4_Are_Operators(string input, int expectedOp, int expectedVersion, int expectedValue)
        {
            var bitString = ConvertToBits(input);

            var actualNode = parser.Parse(bitString);

            Assert.That(
                actualNode,
                Has.Property("Id").Matches<NodeId>(x => x.TypeId == expectedOp)
            );

            var value = actualNode.Evaluate();
            Assert.AreEqual(expectedValue, value);
        }

        [Test]
        [TestCase("8A004A801A8002F478", 16)]
        [TestCase("620080001611562C8802118E34", 12)]
        public void Has_Versions_We_Can_Sum(string input, int expectedVersionSum)
        {
            var node = parser.Parse(ConvertToBits(input));
            var actual = node.VersionSum();
            Assert.AreEqual(expectedVersionSum, actual);
            
        }

        [Test]
        [TestCase("0 sums", "C200B40A82", 3)]
        [TestCase("1 multiplies", "04005AC33890", 54)]
        [TestCase("2 finds minimum", "880086C3E88112", 7)]
        [TestCase("3 finds maximum", "CE00C43D881120", 9)]
        [TestCase("6 finds less than", "D8005AC2A8F0", 1)]
        [TestCase("5 finds (not) greater than", "F600BC2D8F", 0)]
        [TestCase("7 finds (not) equals to", "9C005AC2F8F0", 0)]
        [TestCase("1 + 3 = 2 * 2", "9C0141080250320F1802104A08", 1)]
        public void Executes_Different_Operations(string description, string input, int expectedValue)
        {
            var node = parser.Parse(ConvertToBits(input));
            var actual = node.Evaluate();
            Assert.AreEqual(expectedValue, actual);
        }

        [Test]
        public void Solves_Puzzle_1()
        {
            const string input = "620D79802F60098803B10E20C3C1007A2EC4C84136F0600BCB8AD0066E200CC7D89D0C4401F87104E094FEA82B0726613C6B692400E14A305802D112239802125FB69FF0015095B9D4ADCEE5B6782005301762200628012E006B80162007B01060A0051801E200528014002A118016802003801E2006100460400C1A001AB3DED1A00063D0E25771189394253A6B2671908020394359B6799529E69600A6A6EB5C2D4C4D764F7F8263805531AA5FE8D3AE33BEC6AB148968D7BFEF2FBD204CA3980250A3C01591EF94E5FF6A2698027A0094599AA471F299EA4FBC9E47277149C35C88E4E3B30043B315B675B6B9FBCCEC0017991D690A5A412E011CA8BC08979FD665298B6445402F97089792D48CF589E00A56FFFDA3EF12CBD24FA200C9002190AE3AC293007A0A41784A600C42485F0E6089805D0CE517E3C493DC900180213D1C5F1988D6802D346F33C840A0804CB9FE1CE006E6000844528570A40010E86B09A32200107321A20164F66BAB5244929AD0FCBC65AF3B4893C9D7C46401A64BA4E00437232D6774D6DEA51CE4DA88041DF0042467DCD28B133BE73C733D8CD703EE005CADF7D15200F32C0129EC4E7EB4605D28A52F2C762BEA010C8B94239AAF3C5523CB271802F3CB12EAC0002FC6B8F2600ACBD15780337939531EAD32B5272A63D5A657880353B005A73744F97D3F4AE277A7DA8803C4989DDBA802459D82BCF7E5CC5ED6242013427A167FC00D500010F8F119A1A8803F0C62DC7D200CAA7E1BC40C7401794C766BB3C58A00845691ADEF875894400C0CFA7CD86CF8F98027600ACA12495BF6FFEF20691ADE96692013E27A3DE197802E00085C6E8F30600010882B18A25880352D6D5712AE97E194E4F71D279803000084C688A71F440188FB0FA2A8803D0AE31C1D200DE25F3AAC7F1BA35802B3BE6D9DF369802F1CB401393F2249F918800829A1B40088A54F25330B134950E0";
            var bits = ConvertToBits(input);
            var node = parser.Parse(bits);
            Assert.AreEqual(936, node.VersionSum());
        }

        [Test]
        public void Solves_Puzzle_2()
        {
            const string input = "620D79802F60098803B10E20C3C1007A2EC4C84136F0600BCB8AD0066E200CC7D89D0C4401F87104E094FEA82B0726613C6B692400E14A305802D112239802125FB69FF0015095B9D4ADCEE5B6782005301762200628012E006B80162007B01060A0051801E200528014002A118016802003801E2006100460400C1A001AB3DED1A00063D0E25771189394253A6B2671908020394359B6799529E69600A6A6EB5C2D4C4D764F7F8263805531AA5FE8D3AE33BEC6AB148968D7BFEF2FBD204CA3980250A3C01591EF94E5FF6A2698027A0094599AA471F299EA4FBC9E47277149C35C88E4E3B30043B315B675B6B9FBCCEC0017991D690A5A412E011CA8BC08979FD665298B6445402F97089792D48CF589E00A56FFFDA3EF12CBD24FA200C9002190AE3AC293007A0A41784A600C42485F0E6089805D0CE517E3C493DC900180213D1C5F1988D6802D346F33C840A0804CB9FE1CE006E6000844528570A40010E86B09A32200107321A20164F66BAB5244929AD0FCBC65AF3B4893C9D7C46401A64BA4E00437232D6774D6DEA51CE4DA88041DF0042467DCD28B133BE73C733D8CD703EE005CADF7D15200F32C0129EC4E7EB4605D28A52F2C762BEA010C8B94239AAF3C5523CB271802F3CB12EAC0002FC6B8F2600ACBD15780337939531EAD32B5272A63D5A657880353B005A73744F97D3F4AE277A7DA8803C4989DDBA802459D82BCF7E5CC5ED6242013427A167FC00D500010F8F119A1A8803F0C62DC7D200CAA7E1BC40C7401794C766BB3C58A00845691ADEF875894400C0CFA7CD86CF8F98027600ACA12495BF6FFEF20691ADE96692013E27A3DE197802E00085C6E8F30600010882B18A25880352D6D5712AE97E194E4F71D279803000084C688A71F440188FB0FA2A8803D0AE31C1D200DE25F3AAC7F1BA35802B3BE6D9DF369802F1CB401393F2249F918800829A1B40088A54F25330B134950E0";
            var bits = ConvertToBits(input);
            var node = parser.Parse(bits);
            Assert.AreEqual(6802496672062, node.Evaluate());
        }

        private static string ConvertToBits(string input)
        {
            var hexNibbles = input.ToCharArray();
            var nibbles = hexNibbles.Select(h => int.Parse(h.ToString(), NumberStyles.AllowHexSpecifier));
            var bitString = String.Join("", nibbles.Select(nibble => Convert.ToString(nibble, 2).PadLeft(4, '0')));
            return bitString;
        }
    }

    public record NodeId(int Version, int TypeId);

    public abstract record Node(NodeId Id)
    {
        public abstract long Evaluate();
        public abstract int VersionSum();
    }

    public record Literal(NodeId Id, long Value) : Node(Id)
    {
        public override long Evaluate() => Value;
        public override int VersionSum() => Id.Version;
    };

    public abstract record BinaryOperation(NodeId Id, Node Left, Node Right) : Node(Id)
    {
        public override int VersionSum() => Id.Version + Left.VersionSum() + Right.VersionSum();
    }

    public abstract record CompositeOperation(NodeId Id, Node[] Children) : Node(Id)
    {
        public override int VersionSum() => Id.Version + Children.Sum(x => x.VersionSum());
    }

    public record Sum(NodeId Id, Node[] Children) : CompositeOperation(Id, Children)
    {
        public override long Evaluate() => Children.Sum(x => x.Evaluate());
    }

    public record Product(NodeId Id, Node[] Children) : CompositeOperation(Id, Children)
    {
        public override long Evaluate() => Children.Aggregate((long)1, (prev, cur)  => prev * cur.Evaluate());
    }

    public record Minimum(NodeId Id, Node[] Children) : CompositeOperation(Id, Children)
    {
        public override long Evaluate() => Children.Min(x => x.Evaluate());
    }

    public record Maximum(NodeId Id, Node[] Children) : CompositeOperation(Id, Children)
    {
        public override long Evaluate() => Children.Max(x => x.Evaluate());
    }

    public record GreaterThan(NodeId Id, Node Left, Node Right) : BinaryOperation(Id, Left, Right)
    {
        public override long Evaluate() => Left.Evaluate() > Right.Evaluate() ? 1 : 0;
    }

    public record LessThan(NodeId Id, Node Left, Node Right) : BinaryOperation(Id, Left, Right)
    {
        public override long Evaluate() => Left.Evaluate() < Right.Evaluate() ? 1 : 0;
    }

    public record Equal(NodeId Id, Node Left, Node Right) : BinaryOperation(Id, Left, Right)
    {
        public override long Evaluate() => Left.Evaluate() == Right.Evaluate() ? 1 : 0;
    }

    public class NibbleParser
    {
        const int MoreGroupsMask = 0b10000;

        private int index;
        private string bitString;

        private Dictionary<int, Func<Node>> parsers;
        private NodeId currentId;

        private Dictionary<int, Func<NodeId, Node[], Node>> factories = new Dictionary<int, Func<NodeId, Node[], Node>>
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

            var totalBitString = nibbles.Aggregate("", (str, c) => str + Convert.ToString(c, 2).PadLeft(4, '0'));
            var total = Convert.ToInt64(totalBitString, 2);
            return new Literal(currentId, total);
        }

        private int NextBits(int length)
        {
            var bits = Convert.ToInt32(bitString.Substring(index, length), 2);
            index += length;
            return bits;
        }
    }
}