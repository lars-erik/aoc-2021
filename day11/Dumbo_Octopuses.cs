using System;
using System.Runtime.CompilerServices;
using common;
using NUnit.Framework;

namespace day11
{
    [TestFixture]
    public class Dumbo_Octopuses
    {
        private int[,] matrix;
        private int height;
        private int width;
        private int[,] newMatrix;
        private bool[,] flashes;
        private int currentFlashCount;
        private int flashCount;

        [SetUp]
        public void Setup()
        {
            var lines = Resources.GetResourceLines(typeof(Dumbo_Octopuses), TestContext.CurrentContext.Test.Arguments[0]!.ToString());
            height = lines.Length;
            width = lines[0].Length;
            matrix = new int[height, width];
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    matrix[y, x] = Convert.ToInt32(new String(lines[y][x], 1));
                }
            }

            currentFlashCount = 0;
            flashCount = 0;
        }

        [Test]
        [TestCase("day11.sample.txt", 1656)]
        [TestCase("day11.input.txt", 1655)]
        public void Flash_When_At_Or_Above_10(string resource, int expectedFlashes)
        {
            var steps = 100;

            Console.WriteLine("Before any steps:");
            DumpMatrix(matrix);

            for (var step = 0; step < steps; step++)
            {
                currentFlashCount = 0;
                newMatrix = new int[height, width];
                flashes = new bool[height, width];
                IterateMatrix((y, x) => newMatrix[y, x] = matrix[y, x] + 1);

                IterateMatrix(IncreaseNeighbours);

                matrix = newMatrix;
                IterateMatrix((y, x) =>
                {
                    if (matrix[y, x] > 9) matrix[y, x] = 0;
                });
                flashCount += currentFlashCount;

                if (step < 10 || (step % 10) == 9)
                { 
                    Console.WriteLine($"After step {step + 1} with {flashCount}:");
                    DumpMatrix(matrix);
                }
            }

            Assert.AreEqual(expectedFlashes, flashCount);
        }

        [Test]
        [TestCase("day11.sample.txt", 195)]
        [TestCase("day11.input.txt", 337)]
        public void Flashes_Simultaneously_At_Given_Intervals(string resource, int expectedStep)
        {
            Console.WriteLine("Before any steps:");
            DumpMatrix(matrix);

            flashCount = 0;
            int step;
            for (step = 0; currentFlashCount < 100; step++)
            {
                currentFlashCount = 0;
                newMatrix = new int[height, width];
                flashes = new bool[height, width];
                IterateMatrix((y, x) => newMatrix[y, x] = matrix[y, x] + 1);

                IterateMatrix(IncreaseNeighbours);

                matrix = newMatrix;
                IterateMatrix((y, x) =>
                {
                    if (matrix[y, x] > 9) matrix[y, x] = 0;
                });
                flashCount += currentFlashCount;

                if (step < 10 || (step % 10) == 9)
                { 
                    Console.WriteLine($"After step {step + 1} with {flashCount}:");
                    DumpMatrix(matrix);
                }
            }

            Assert.AreEqual(expectedStep, step);
        }

        private void IncreaseNeighbours(int y, int x)
        {
            if (newMatrix[y, x] > 9 && !flashes[y, x])
            {
                flashes[y, x] = true;
                currentFlashCount++;
                IterateNeighbours(y, x, (y2, x2) =>
                {
                    if (newMatrix[y2, x2] <= 9)
                    {
                        newMatrix[y2, x2]++;
                        IncreaseNeighbours(y2, x2);
                    }
                });
            }
        }


        private void DumpMatrix(int[,] matrixToDump)
        {
            IterateMatrix((y, x) =>
            {
                Console.Write($"{matrixToDump[y, x]}");
                if (x == width - 1) Console.WriteLine();
            });
            Console.WriteLine();
        }

        private void IterateMatrix(Action<int, int> action)
        {
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    action(y, x);
                }
            }

        }

        private void IterateNeighbours(int y, int x, Action<int, int> action)
        {
            for (var y2 = Math.Max(y - 1, 0); y2 <= Math.Min(y + 1, height - 1); y2++)
            {
                for (var x2 = Math.Max(x - 1, 0); x2 <= Math.Min(x + 1, width - 1); x2++)
                {
                    if (!(y2 == y && x2 == x))
                    { 
                        action(y2, x2);
                    }
                }
            }

        }
    }
}