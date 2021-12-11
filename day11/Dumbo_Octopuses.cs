using System;
using System.Runtime.CompilerServices;
using common;
using NUnit.Framework;

namespace day11
{
    [TestFixture]
    public class Dumbo_Octopuses
    {
        private DumboOctopuses octos;

        [SetUp]
        public void Setup()
        {
            var lines = Resources.GetResourceLines(typeof(Dumbo_Octopuses), TestContext.CurrentContext.Test.Arguments[0]!.ToString());
            octos = DumboOctopuses.Create(lines);
        }

        [Test]
        [TestCase("day11.sample.txt", 1656)]
        [TestCase("day11.input.txt", 1655)]
        public void Flash_When_At_Or_Above_10(string resource, int expectedFlashes)
        {
            var steps = 100;

            Console.WriteLine("Before any steps:");
            DumpMatrix(octos.Matrix);

            for (var step = 0; step < steps; step++)
            {
                octos.PassTime();

                if (step < 10 || (step % 10) == 9)
                { 
                    Console.WriteLine($"After step {step + 1} with {octos.FlashCount}:");
                    DumpMatrix(octos.Matrix);
                }
            }

            Assert.AreEqual(expectedFlashes, octos.FlashCount);
        }

        [Test]
        [TestCase("day11.sample.txt", 195)]
        [TestCase("day11.input.txt", 337)]
        public void Flashes_Simultaneously_At_Given_Intervals(string resource, int expectedStep)
        {
            Console.WriteLine("Before any steps:");
            DumpMatrix(octos.Matrix);

            int step;
            for (step = 0; octos.CurrentFlashCount < 100; step++)
            {
                octos.PassTime();

                if (step < 10 || (step % 10) == 9)
                { 
                    Console.WriteLine($"After step {step + 1} with {octos.FlashCount}:");
                    DumpMatrix(octos.Matrix);
                }
            }

            Assert.AreEqual(expectedStep, step);
        }


        private void DumpMatrix(int[,] matrixToDump)
        {
            octos.IterateMatrix((y, x) =>
            {
                Console.Write($"{matrixToDump[y, x]}");
                if (x == octos.Width - 1) Console.WriteLine();
            });
            Console.WriteLine();
        }
    }

    public class DumboOctopuses
    {
        private int[,] matrix;
        private int[,] newMatrix;
        private bool[,] flashes;

        public int Height { get; }

        public int Width { get; }

        public int CurrentFlashCount { get; private set; }

        public int FlashCount { get; private set; }

        public int[,] Matrix => matrix;

        public int[,] NewMatrix => newMatrix;

        public DumboOctopuses(int width, int height)
        {
            Width = width;
            Height = height;
            CurrentFlashCount = 0;
            FlashCount = 0;
        }

        public void PassTime()
        {
            CurrentFlashCount = 0;
            newMatrix = new int[Height, Width];
            flashes = new bool[Height, Width];
            IterateMatrix((y, x) => newMatrix[y, x] = matrix[y, x] + 1);

            IterateMatrix(IncreaseNeighbours);

            matrix = newMatrix;
            IterateMatrix((y, x) =>
            {
                if (matrix[y, x] > 9) matrix[y, x] = 0;
            });
            FlashCount += CurrentFlashCount;
        }

        private void IncreaseNeighbours(int y, int x)
        {
            if (newMatrix[y, x] > 9 && !flashes[y, x])
            {
                flashes[y, x] = true;
                CurrentFlashCount++;
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

        public void IterateMatrix(Action<int, int> action)
        {
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    action(y, x);
                }
            }

        }

        private void IterateNeighbours(int y, int x, Action<int, int> action)
        {
            for (var y2 = Math.Max(y - 1, 0); y2 <= Math.Min(y + 1, Height - 1); y2++)
            {
                for (var x2 = Math.Max(x - 1, 0); x2 <= Math.Min(x + 1, Width - 1); x2++)
                {
                    if (!(y2 == y && x2 == x))
                    { 
                        action(y2, x2);
                    }
                }
            }
        }

        public static DumboOctopuses Create(string[] lines)
        {
            var octos = new DumboOctopuses(lines.Length, lines[0].Length);
            octos.matrix = new int[octos.Height, octos.Width];
            for (var y = 0; y < octos.Height; y++)
            {
                for (var x = 0; x < octos.Width; x++)
                {
                    octos.matrix[y, x] = Convert.ToInt32(new String(lines[y][x], 1));
                }
            }

            return octos;
        }
    }
}