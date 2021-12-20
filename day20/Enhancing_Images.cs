using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using ApprovalTests;
using ApprovalTests.Reporters;
using common;
using NUnit.Framework;

namespace day20
{
    [TestFixture]
    [UseReporter(typeof(VisualStudioReporter))]
    public class Enhancing_Images
    {
        private string alg;
        private List<Vector2> points;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Expands_Sample_Image_Twice()
        {
            const int steps = 2;

            const int expectedPixels = 35;
            var input = @"..#.#..#####.#.#.#.###.##.....###.##.#..###.####..#####..#....#..#..##..###..######.###...####..#..#####..##..#.#####...##.#.#..#.##..#.#......#.###.######.###.####...#.##.##..#..#..#####.....#.#....###..#.##......#.....#..#..#..##..#...##.######.####.####.#.#...#.......#..#.#.#...####.##.#......#..#...##.#.##..#...##.#.##..###.#......#.#.......#.#.#.####.###.##...#.....####.#..#..#.##.#....##..#.####....##...##..#...#......#.#.......#.......##..####..#...#.#.#...##..#.#..###..#####........#..####......#..#

#..#.
#....
##..#
..#..
..###";
            var lines = input.Replace("\r", "").Split('\n');

            alg = lines[0];
            points = ParsePoints(lines);

            var builder = new StringBuilder();

            WritePoints(points, builder);

            for (var step = 0; step < steps; step++)
            {
                var newPoints = Enhance(points, alg);

                points = newPoints;
                WritePoints(points, builder);
            }

            Approvals.Verify(builder.ToString());
            //Assert.AreEqual(expectedPixels, points.Count);
        }

        [Test]
        public void Enhances_Input()
        {
            const int steps = 2;

            const int expectedPixels = 35;
            var lines = Resources.GetResourceLines(typeof(Enhancing_Images), "day20.input.txt");

            alg = lines[0];
            points = ParsePoints(lines);

            WritePoints(points);

            for (var step = 0; step < steps; step++)
            {
                var newPoints = Enhance(points, alg);

                points = newPoints;
                WritePoints(points);
            }

            var builder = new StringBuilder();
            WritePoints(points, builder);

            // 7240?
            Assert.AreEqual(expectedPixels, points.Count);
            Approvals.Verify(builder.ToString());
        }

        private static List<Vector2> Enhance(List<Vector2> points, string alg)
        {
            var width = (int) MathF.Ceiling(points.Max(p => p.X) - points.Min(p => p.X)) + 4;
            var height = (int) MathF.Ceiling(points.Max(p => p.Y) - points.Min(p => p.Y)) + 4;
            var minX = width / -2 - 2;
            var maxX = width / 2 + 2;
            var minY = height / -2 - 2;
            var maxY = height / 2 + 2;
            var newPoints = new List<Vector2>();
            for (var y = minY - 1; y < maxY + 1; y++)
            {
                for (var x = minX - 1; x < maxX + 1; x++)
                {
                    var pointStr = "";
                    for (var y2 = -1; y2 <= 1; y2++)
                    {
                        for (var x2 = -1; x2 <= 1; x2++)
                        {
                            pointStr += points.Contains(new Vector2(x + x2, y + y2)) ? "1" : "0";
                        }
                    }

                    var pos = Convert.ToInt32(pointStr, 2);
                    var charAt = alg[pos];
                    if (charAt == '#')
                    {
                        newPoints.Add(new Vector2(x, y));
                    }
                }
            }

            return newPoints;
        }

        private static List<Vector2> ParsePoints(string[] lines)
        {
            var  points = new List<Vector2>();

            var width = lines[2].Length;
            var height = lines.Length - 2;
            var minX = width / -2;
            var minY = height / -2;

            for (int i = 2, y = minY; i < lines.Length; i++, y++)
            {
                var line = lines[i];
                for (int j = 0, x = minX; j < line.Length; j++, x++)
                {
                    if (line[j] == '#')
                    {
                        points.Add(new Vector2(x, y));
                    }
                }
            }

            return points;
        }

        private static void WritePoints(List<Vector2> points, StringBuilder builder = null)
        {
            var width = (int) MathF.Ceiling(points.Max(p => p.X) - points.Min(p => p.X));
            var height = (int) MathF.Ceiling(points.Max(p => p.Y) - points.Min(p => p.Y));
            if (width % 2 == 0) width++;
            if (height % 2 == 0) height++;
            width = Math.Max(width, 15);
            height = Math.Max(height, 15);
            var minX = width / -2;
            var maxX = width / 2;
            var minY = height / -2;
            var maxY = height / 2;

            Action<string> output = builder != null ? (s) => builder.Append(s) : Console.Write;

            for (var y = minY; y <= maxY; y++)
            {
                for (var x = minX; x <= maxX; x++)
                {
                    if (points.Contains(new Vector2(x, y)))
                    {
                        output("#");
                    }
                    else
                    {
                        output(".");
                    }
                }

                output("\r\n");
            }

            output("\r\n");
        }
    }
}