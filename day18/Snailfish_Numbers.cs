using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApprovalTests;
using ApprovalTests.Reporters;
using common;
using NUnit.Framework;

namespace day18
{
    [TestFixture]
    [UseReporter(typeof(VisualStudioReporter))]
    public class Snailfish_Numbers
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase("[[[[[9,8],1],2],3],4]", "[[[[0,9],2],3],4]")]
        [TestCase("[7,[6,[5,[4,[3,2]]]]]", "[7,[6,[5,[7,0]]]]")]
        [TestCase("[[6,[5,[4,[3,2]]]],1]", "[[6,[5,[7,0]]],3]")]
        [TestCase("[[3,[2,[1,[7,3]]]],[6,[5,[4,[3,2]]]]]", "[[3,[2,[8,0]]],[9,[5,[4,[3,2]]]]]")]
        [TestCase("[[3,[2,[8,0]]],[9,[5,[4,[3,2]]]]]", "[[3,[2,[8,0]]],[9,[5,[7,0]]]]")]
        public void Are_Reduced_By_Exploding(string input, string expectedReduced)
        {
            var node = Parse(input);
            var reduced = node.TryExplode(1);
            var result = node.ToString();
            Console.WriteLine(result);

            Assert.AreEqual(expectedReduced, result);
        }

        [Test]
        public void Are_Exploded_And_Split_After_Adding()
        {
            const string expected = "[[[[0,7],4],[[7,8],[6,0]]],[8,1]]";
            var a = (PairNode)Parse("[[[[4,3],4],4],[7,[[8,4],9]]]");
            var b = (PairNode)Parse("[1,1]");

            var num = a.Add(b);
            num.TotalReduce();

            Console.WriteLine(num);


            Assert.AreEqual(expected, num.ToString());
        }

        [Test]
        public void Input_Has_Max_5_Levels()
        {
            var lines = Resources.GetResourceLines(typeof(Snailfish_Numbers), "day18.input.txt");
            var nodes = lines.Select(Parse);
            var maxLevel = nodes.Max(x => MaxLevel(x));
            Assert.AreEqual(5, maxLevel);
        }

        [Test]
        [TestCase("[[[[0,7],4],[[7,8],[6,0]]],[8,1]]", 1384)]
        public void Have_Magnitude(string input, int expectedMagnitude)
        {
            var node = Parse(input);
            Assert.AreEqual(expectedMagnitude, node.Magnitude);
        }

        [Test]
        [TestCase(4, "[[[[1,1],[2,2]],[3,3]],[4,4]]")]
        [TestCase(5, "[[[[3,0],[5,3]],[4,4]],[5,5]]")]
        [TestCase(6, "[[[[5,0],[7,4]],[5,5]],[6,6]]")]
        public void Are_Summed_In_Mysterious_Ways(int range, string expected)
        {
            var pairs = Enumerable.Range(1, range)
                .Select(x => new PairNode(new LiteralSNode(x), new LiteralSNode(x)))
                .ToArray();
            var num = pairs[0];
            for (var i = 1; i < pairs.Length; i++)
            {
                num = num.Add(pairs[i]);
                num.TotalReduce();
            }
            Console.WriteLine(num);
            Assert.AreEqual(expected, num.ToString());
        }

        [Test]
        public void Solves_Sample_Puzzle()
        {
            var lines = @"[[[0,[4,5]],[0,0]],[[[4,5],[2,6]],[9,5]]]
[7,[[[3,7],[4,3]],[[6,3],[8,8]]]]
[[2,[[0,8],[3,4]]],[[[6,7],1],[7,[1,6]]]]
[[[[2,4],7],[6,[0,5]]],[[[6,8],[2,8]],[[2,1],[4,5]]]]
[7,[5,[[3,8],[1,4]]]]
[[2,[2,2]],[8,[8,1]]]
[2,9]
[1,[[[9,3],9],[[9,0],[0,7]]]]
[[[5,[7,4]],7],1]
[[[[4,2],2],6],[8,7]]".Replace("\r", "").Split("\n", StringSplitOptions.RemoveEmptyEntries);

            var num = (PairNode)Parse(lines[0]);

            var builder = new StringBuilder();

            for (var i = 1; i < lines.Length; i++)
            {
                var nodeToAdd = (PairNode)Parse(lines[i]);

                builder.AppendLine("  " + num);
                builder.AppendLine("+ " + nodeToAdd);

                num = num.Add(nodeToAdd);

                num.TotalReduce();

                builder.AppendLine("= " + num);
                builder.AppendLine();
            }

            Console.WriteLine(builder.ToString());
            Approvals.Verify(builder.ToString());

            Assert.AreEqual(3488, num.Magnitude);
        }

        [Test]
        public void Solves_Puzzle_1()
        {
            var lines = Resources.GetResourceLines(typeof(Snailfish_Numbers), "day18.input.txt");
            var num = (PairNode)Parse(lines[0]);

            for (var i = 1; i < lines.Length; i++)
            {
                var nodeToAdd = (PairNode)Parse(lines[i]);

                Console.WriteLine(num + " + ");
                Console.WriteLine(nodeToAdd + " = ");

                num = num.Add(nodeToAdd);

                num.TotalReduce();

                Console.WriteLine(num);
                Console.WriteLine();
            }

            Assert.AreEqual(4137, num.Magnitude);
        }

        [Test]
        public void Solves_Puzzle_2_Sample()
        {
            var lines = @"[[[0,[5,8]],[[1,7],[9,6]]],[[4,[1,2]],[[1,4],2]]]
[[[5,[2,8]],4],[5,[[9,9],0]]]
[6,[[[6,2],[5,6]],[[7,6],[4,7]]]]
[[[6,[0,7]],[0,9]],[4,[9,[9,0]]]]
[[[7,[6,4]],[3,[1,3]]],[[[5,5],1],9]]
[[6,[[7,3],[3,2]]],[[[3,8],[5,7]],4]]
[[[[5,4],[7,7]],8],[[8,3],8]]
[[9,3],[[9,9],[6,[4,9]]]]
[[2,[[7,7],7]],[[5,8],[[9,3],[0,2]]]]
[[[[5,2],5],[8,[3,7]]],[[5,[7,5]],[4,4]]]".Replace("\r", "").Split("\n");

            var mags = new List<int>();
            for (var i = 0; i<lines.Length; i++)
            {
                var num = (PairNode)Parse(lines[i]);

                for (var j = 0; j < lines.Length; j++)
                {
                    if (j == i) continue;

                    var numToAdd = (PairNode)Parse(lines[j]);
                    var result = num.Add(numToAdd);
                    result.TotalReduce();

                    mags.Add(result.Magnitude);
                }
            }
            Assert.AreEqual(3993, mags.Max());
        }

        [Test]
        public void Solves_Puzzle_2()
        {
            var lines = Resources.GetResourceLines(typeof(Snailfish_Numbers), "day18.input.txt");

            var mags = new List<int>();
            for (var i = 0; i<lines.Length; i++)
            {
                for (var j = 0; j < lines.Length; j++)
                {
                    if (j == i) continue;

                    // TODO: really! Stop mutating these numbers. 🤦‍
                    var num = (PairNode)Parse(lines[i]);
                    var numToAdd = (PairNode)Parse(lines[j]);
                    var result = num.Add(numToAdd);
                    result.TotalReduce();

                    mags.Add(result.Magnitude);
                }
            }
            Assert.AreEqual(4573, mags.Max());
        }

        private int MaxLevel(SnailfishNode node, int currentMax = 1, int level = 0)
        {
            level += 1;
            if (node is PairNode pair)
            {
                var leftMax = MaxLevel(pair.Left, currentMax, level);
                var rightMax = MaxLevel(pair.Right, currentMax, level);
                currentMax = Math.Max(currentMax, Math.Max(leftMax, rightMax));
            }
            else
            {
                currentMax = Math.Max(level, currentMax);
            }
            return currentMax;
        }

        private SnailfishNode Parse(string input)
        {
            this.input = input;
            index = 0;
            var node = ParseNext();
            return node;
        }

        private int index = 0;
        private string input;
        private LiteralSNode previous;

        private SnailfishNode ParseNext()
        {
            if (input[index] == '[')
            {
                index++;
                var left = ParseNext();
                index++;
                var right = ParseNext();
                index++;
                var node = new PairNode(left, right);
                return node;
            }
            else
            {
                var literalSNode = new LiteralSNode(int.Parse("" + input[index]))
                {
                    Previous = previous
                };
                if (previous != null) previous.Next = literalSNode;
                previous = literalSNode;
                index++;
                return literalSNode;
            }
        }
    }

    public abstract class SnailfishNode
    {
        public abstract int Magnitude { get; }

        public void TotalReduce(StringBuilder builder = null)
        {
            bool reduced = true;
            builder?.AppendLine("Reducing: ".PadRight(15, ' ') + this);
            int index = 0;
            while (reduced)
            {
                reduced = TryExplode(1);
                if (reduced)
                {
                    builder?.AppendLine(("Exploded " + (++index) + ":  ").PadRight(15, ' ') + this);
                }
                if (!reduced)
                { 
                    reduced = TrySplit();
                    if (reduced)
                    {
                        builder?.AppendLine(("Split " + (++index) + ":  ").PadRight(15, ' ') + this);
                    }
                }
            }
        }

        public abstract bool TryExplode(int level);
        public abstract bool TrySplit();
    }

    class LiteralSNode : SnailfishNode
    {
        public override int Magnitude => Value;

        public int Value { get; set; }

        public LiteralSNode(int value)
        {
            Value = value;
        }

        public LiteralSNode Previous { get; set; }
        public LiteralSNode Next { get; set; }

        public override string ToString()
        {
            return Value.ToString();
        }

        public PairNode Split()
        {
            var prev = Previous;
            var next = Next;
            var newLeft = new LiteralSNode((int)Math.Floor((double)Value / 2));
            var newRight = new LiteralSNode((int)Math.Ceiling((double)Value / 2));
            var newNode = new PairNode(newLeft, newRight);
            newLeft.Previous = prev;
            if (prev != null) prev.Next = newLeft;
            newLeft.Next = newRight;
            newRight.Previous = newLeft;
            newRight.Next = next;
            if (next != null) next.Previous = newRight;
            return newNode;
        }

        public override bool TryExplode(int level)
        {
            return false;
        }
        public override bool TrySplit()
        {
            return false;
        }
    }

    class PairNode : SnailfishNode
    {
        public override int Magnitude => Left.Magnitude * 3 + Right.Magnitude * 2;

        public LiteralSNode FirstLiteral => Left is LiteralSNode firstLiteral ? firstLiteral : ((PairNode)Left).FirstLiteral;
        public LiteralSNode LastLiteral => Right is LiteralSNode lastLiteral ? lastLiteral : ((PairNode)Right).LastLiteral;

        public SnailfishNode Left { get; set; }
        public SnailfishNode Right { get; set; }

        public PairNode(SnailfishNode left, SnailfishNode right)
        {
            Left = left;
            Right = right;
        }
        public override string ToString()
        {
            return $"[{Left},{Right}]";
        }

        public PairNode Add(PairNode b)
        {
            var num = new PairNode(this, b);
            LastLiteral.Next = b.FirstLiteral;
            b.FirstLiteral.Previous = LastLiteral;
            return num;
        }

        public LiteralSNode Explode()
        {
            var leftLiteral = (LiteralSNode)Left;
            var rightLiteral = (LiteralSNode)Right;

            var newNode = new LiteralSNode(0);
            if (leftLiteral.Previous != null)
            {
                leftLiteral.Previous.Value += leftLiteral.Value;
                leftLiteral.Previous.Next = newNode;
                newNode.Previous = leftLiteral.Previous;
            }

            if (rightLiteral.Next != null)
            {
                rightLiteral.Next.Value += rightLiteral.Value;
                rightLiteral.Next.Previous = newNode;
                newNode.Next = rightLiteral.Next;
            }

            return newNode;
        }

        public override bool TryExplode(int level)
        {
            if (level >= 4)
            {
                if (Left is PairNode leftPair)
                {
                    Left = leftPair.Explode();
                    return true;
                }

                if (Right is PairNode rightPair)
                {
                    Right = rightPair.Explode();
                    return true;
                }
            }

            return Left.TryExplode(level + 1)
                   || Right.TryExplode(level + 1);
        }

        public override bool TrySplit()
        {
            if (Left is LiteralSNode { Value: > 9 } leftLit)
            {
                Left = leftLit.Split();
                return true;
            }

            if (Left.TrySplit())
            {
                return true;
            }

            if (Right is LiteralSNode { Value: > 9 } rightLit)
            {
                Right = rightLit.Split();
                return true;
            }

            return Right.TrySplit();
        }
    }

}