using System;
using System.Linq;
using common;
using NUnit.Framework;

namespace day18
{
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
            var reduced = Reduce(node);
            var result = node.ToString();
            Console.WriteLine(result);

            Assert.AreEqual(expectedReduced, result);
        }

        [Test]
        public void Input_Has_Max_5_Levels()
        {
            var lines = Resources.GetResourceLines(typeof(Snailfish_Numbers), "day18.input.txt");
            var nodes = lines.Select(Parse);
            var maxLevel = nodes.Max(x => MaxLevel(x));
            Assert.AreEqual(5, maxLevel);
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

        private bool Reduce(SnailfishNode node, int level = 1)
        {
            if (node is PairNode pairNode)
            {
                if (level >= 4)
                {
                    if (pairNode.Left is PairNode leftPair)
                    {
                        var leftLiteral = (LiteralSNode)leftPair.Left;
                        var rightLiteral = (LiteralSNode)leftPair.Right;

                        if (leftLiteral.Previous != null)
                        {
                            leftLiteral.Previous.Value += leftLiteral.Value;
                        }
                        if (rightLiteral.Next != null)
                        {
                            rightLiteral.Next.Value += rightLiteral.Value;
                        }

                        pairNode.Left = new LiteralSNode(0);
                        return true;

                    }

                    if (pairNode.Right is PairNode rightPair)
                    {
                        var leftLiteral = (LiteralSNode)rightPair.Left;
                        var rightLiteral = (LiteralSNode)rightPair.Right;

                        if (leftLiteral.Previous != null)
                        {
                            leftLiteral.Previous.Value += leftLiteral.Value;
                        }
                        if (rightLiteral.Next != null)
                        {
                            rightLiteral.Next.Value += rightLiteral.Value;
                        }

                        pairNode.Right= new LiteralSNode(0);
                        return true;
                    }

                    return false;
                }

                return Reduce(pairNode.Left, level + 1)
                    || Reduce(pairNode.Right, level + 1);
            }

            return false;
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

    abstract class SnailfishNode
    {
    }

    class LiteralSNode : SnailfishNode
    {
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
    }

    class PairNode : SnailfishNode
    {
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
    }

}