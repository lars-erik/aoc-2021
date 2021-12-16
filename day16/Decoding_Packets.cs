using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;

namespace day16
{
    public class Decoding_Packets
    {
        private const string PuzzleInput = "620D79802F60098803B10E20C3C1007A2EC4C84136F0600BCB8AD0066E200CC7D89D0C4401F87104E094FEA82B0726613C6B692400E14A305802D112239802125FB69FF0015095B9D4ADCEE5B6782005301762200628012E006B80162007B01060A0051801E200528014002A118016802003801E2006100460400C1A001AB3DED1A00063D0E25771189394253A6B2671908020394359B6799529E69600A6A6EB5C2D4C4D764F7F8263805531AA5FE8D3AE33BEC6AB148968D7BFEF2FBD204CA3980250A3C01591EF94E5FF6A2698027A0094599AA471F299EA4FBC9E47277149C35C88E4E3B30043B315B675B6B9FBCCEC0017991D690A5A412E011CA8BC08979FD665298B6445402F97089792D48CF589E00A56FFFDA3EF12CBD24FA200C9002190AE3AC293007A0A41784A600C42485F0E6089805D0CE517E3C493DC900180213D1C5F1988D6802D346F33C840A0804CB9FE1CE006E6000844528570A40010E86B09A32200107321A20164F66BAB5244929AD0FCBC65AF3B4893C9D7C46401A64BA4E00437232D6774D6DEA51CE4DA88041DF0042467DCD28B133BE73C733D8CD703EE005CADF7D15200F32C0129EC4E7EB4605D28A52F2C762BEA010C8B94239AAF3C5523CB271802F3CB12EAC0002FC6B8F2600ACBD15780337939531EAD32B5272A63D5A657880353B005A73744F97D3F4AE277A7DA8803C4989DDBA802459D82BCF7E5CC5ED6242013427A167FC00D500010F8F119A1A8803F0C62DC7D200CAA7E1BC40C7401794C766BB3C58A00845691ADEF875894400C0CFA7CD86CF8F98027600ACA12495BF6FFEF20691ADE96692013E27A3DE197802E00085C6E8F30600010882B18A25880352D6D5712AE97E194E4F71D279803000084C688A71F440188FB0FA2A8803D0AE31C1D200DE25F3AAC7F1BA35802B3BE6D9DF369802F1CB401393F2249F918800829A1B40088A54F25330B134950E0";
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

            var bitString = NibbleParser.ConvertToBits(input);
            Console.WriteLine(bitString);

            var actual = ((Literal)parser.Parse(bitString)).Value;

            Assert.AreEqual(expected, actual);

        }

        [Test]
        [TestCase("38006F45291200", 6, 2, 1)]
        [TestCase("EE00D40C823060", 3, 3, 3)]
        public void Of_TypeIds_Other_Than_4_Are_Operators(string input, int expectedOp, int expectedVersion, int expectedValue)
        {
            var bitString = NibbleParser.ConvertToBits(input);

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
            var node = parser.Parse(NibbleParser.ConvertToBits(input));
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
            var node = parser.Parse(NibbleParser.ConvertToBits(input));
            var actual = node.Evaluate();
            Assert.AreEqual(expectedValue, actual);
        }

        [Test]
        public void Solves_Puzzle_1()
        {
            var bits = NibbleParser.ConvertToBits(PuzzleInput);
            var node = parser.Parse(bits);
            Assert.AreEqual(936, node.VersionSum());
        }

        [Test]
        public void Solves_Puzzle_2()
        {
            var bits = NibbleParser.ConvertToBits(PuzzleInput);
            var node = parser.Parse(bits);
            Assert.AreEqual(6802496672062, node.Evaluate());
        }

        [Test]
        [TestCase("With meta", true)]
        [TestCase("W/O meta", false)]
        public void Prints_Nice_Syntax_Tree(string description, bool drawMeta)
        {
            var bits = NibbleParser.ConvertToBits(PuzzleInput);
            var node = parser.Parse(bits);

            var builder = new StringBuilder();
            SyntaxTreeAsciiVisualizer.PrintNode(node, builder, drawMeta);

            Console.WriteLine(builder.ToString());
        }

        [Test]
        public void Prints_Nice_DSL()
        {
            var bits = NibbleParser.ConvertToBits(PuzzleInput);
            var node = parser.Parse(bits);

            var builder = new StringBuilder();
            DslVisualizer.VisualizeDsl(node, builder);

            Console.WriteLine(builder.ToString());
        }

        private const string plantUmlStyling = @"
<style>

node {
    LineColor #22FF22 
    BackgroundColor #333333
    FontColor #22FF22
    FontName Lucida Console
    FontSize 14
    HorizontalAlignment center
    Padding 10
}

arrow {
    LineColor #22FF22
    
}
</style>
";

        [Test]
        public void Prints_Nice_UML_MindMap()
        {
            var bits = NibbleParser.ConvertToBits(PuzzleInput);
            var node = parser.Parse(bits);

            var builder = new StringBuilder();
            builder.AppendLine("@startmindmap");
            builder.Append(plantUmlStyling);

            VisualizeUml(node, builder);

            builder.AppendLine("@endmindmap");

            Console.WriteLine(builder.ToString());
        }

        [Test]
        public void Prints_Nice_Colorized_UML_MindMap()
        {
            var bits = NibbleParser.ConvertToBits(PuzzleInput);
            var node = parser.Parse(bits);

            var builder = new StringBuilder();
            builder.AppendLine("@startmindmap");
            builder.Append(plantUmlStyling);

            VisualizeColorizedUml(node, builder);

            builder.AppendLine("@endmindmap");

            Console.WriteLine(builder.ToString());
        }

        private void VisualizeUml(Node node, StringBuilder builder, int indent = 1)
        {
            var indentStr = new String('*', indent);
            builder.Append(indentStr);
            builder.Append(" ");
            if (!(node is Literal))
            { 
                builder.Append(node.GetType().Name);
                builder.Append("\\n(");
            }

            builder.Append(node.Evaluate());

            if (!(node is Literal))
            {
                builder.Append(")");
            }

            builder.AppendLine();

            if (node is BinaryOperation binOp)
            {
                VisualizeUml(binOp.Left, builder, indent + 1);
                VisualizeUml(binOp.Right, builder, indent + 1);
            }
            else if (node is CompositeOperation compOp)
            {
                foreach (var child in compOp.Children)
                {
                    VisualizeUml(child, builder, indent + 1);
                }
            }
        }

        private void VisualizeColorizedUml(Node node, StringBuilder builder, int indent = 1)
        {
            var indentStr = new String('*', indent);
            builder.Append(indentStr);
            builder.Append(" ");
            builder.Append(node.Evaluate());

            builder.Append(" <<");
            builder.Append(node.GetType().Name);
            builder.Append(">>");

            builder.AppendLine();

            if (node is BinaryOperation binOp)
            {
                VisualizeColorizedUml(binOp.Left, builder, indent + 1);
                VisualizeColorizedUml(binOp.Right, builder, indent + 1);
            }
            else if (node is CompositeOperation compOp)
            {
                foreach (var child in compOp.Children)
                {
                    VisualizeColorizedUml(child, builder, indent + 1);
                }
            }
        }
    }
}