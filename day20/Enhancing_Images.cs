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
        private List<Point> points;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Expands_Sample_Image_Twice()
        {
            const int expectedPixels = 35;
            var builder = ExecuteSample(2);

            Console.WriteLine(builder.ToString());
            Approvals.Verify(builder.ToString());
            Assert.AreEqual(expectedPixels, points.Count);
        }

        [Test]
        public void Expands_Sample_Image_A_Lot()
        {
            var builder = ExecuteSample(30);

            Console.WriteLine(builder.ToString());
        }

        [Test]
        public void Enhances_Reddit_Input()
        {
            const int steps = 2;

            const int expectedPixels = 5326;
            var lines = Resources.GetResourceLines(typeof(Enhancing_Images), "day20.redditinput.txt");

            alg = lines[0];
            points = ParsePoints(lines);
            var evenState = alg[0] == '#' ? "0" : "1";
            var oddState = alg[0] == '#' ? "1" : "0";

            WritePoints(points, evenState);

            for (var step = 0; step < steps; step++)
            {
                var state = step % 2 == 0 ? evenState : oddState;

                var newPoints = Enhance(points, alg, state);

                points = newPoints;

                WritePoints(points, state);
            }

            var builder = new StringBuilder();
            WritePoints(points, evenState, builder);

            // 7240?
            Assert.AreEqual(expectedPixels, points.Count);
            Approvals.Verify(builder.ToString());
        }

        [Test]
        public void Enhances_Input()
        {
            const int steps = 2;

            const int expectedPixels = 5347;
            var lines = Resources.GetResourceLines(typeof(Enhancing_Images), "day20.input.txt");

            alg = lines[0];
            points = ParsePoints(lines);
            var evenState = alg[0] == '#' ? "0" : "1";
            var oddState = alg[0] == '#' ? "1" : "0";

            WritePoints(points, evenState);

            for (var step = 0; step < steps; step++)
            {
                var state = step % 2 == 0 ? evenState : oddState;

                var newPoints = Enhance(points, alg, state);

                points = newPoints;

                WritePoints(points, state);
            }

            var builder = new StringBuilder();
            WritePoints(points, evenState, builder);

            // 7240?
            Assert.AreEqual(expectedPixels, points.Count);
            Approvals.Verify(builder.ToString());
        }

        [Test]
        public void Enhances_Input_A_Lot()
        {
            const int steps = 50;

            const int expectedPixels = 17172;
            var lines = Resources.GetResourceLines(typeof(Enhancing_Images), "day20.input.txt");

            alg = lines[0];
            points = ParsePoints(lines);
            var evenState = alg[0] == '#' ? "0" : "1";
            var oddState = alg[0] == '#' ? "1" : "0";

            WritePoints(points, evenState);

            for (var step = 0; step < steps; step++)
            {
                var state = step % 2 == 0 ? evenState : oddState;

                var newPoints = Enhance(points, alg, state);

                points = newPoints;

                WritePoints(points, state);
            }

            var builder = new StringBuilder();
            WritePoints(points, evenState, builder);

            // 7240?
            Assert.AreEqual(expectedPixels, points.Count);
            Approvals.Verify(builder.ToString());
        }

        private StringBuilder ExecuteSample(int steps)
        {
            var input =
                @"..#.#..#####.#.#.#.###.##.....###.##.#..###.####..#####..#....#..#..##..###..######.###...####..#..#####..##..#.#####...##.#.#..#.##..#.#......#.###.######.###.####...#.##.##..#..#..#####.....#.#....###..#.##......#.....#..#..#..##..#...##.######.####.####.#.#...#.......#..#.#.#...####.##.#......#..#...##.#.##..#...##.#.##..###.#......#.#.......#.#.#.####.###.##...#.....####.#..#..#.##.#....##..#.####....##...##..#...#......#.#.......#.......##..####..#...#.#.#...##..#.#..###..#####........#..####......#..#

#..#.
#....
##..#
..#..
..###";
            var lines = input.Replace("\r", "").Split('\n');

            alg = lines[0];
            points = ParsePoints(lines);

            var builder = new StringBuilder();

            var evenState = alg[0] == '.' ? "0" : "1";
            var oddState = alg[0] == '.' ? "1" : "0";

            WritePoints(points, evenState, builder);

            for (var step = 1; step <= steps; step++)
            {
                var state = step % 2 == 0 ? evenState : oddState;
                var newPoints = Enhance(points, alg, state);

                points = newPoints;
                WritePoints(points, state, builder);
            }

            return builder;
        }

        private static List<Point> Enhance(List<Point> points, string alg, string outsideState)
        {
            var minX = points.Min(p => p.X);
            var maxX = points.Max(p => p.X);
            var minY = points.Min(p => p.Y);
            var maxY = points.Max(p => p.Y);
            var newPoints = new List<Point>();
            for (var y = minY - 1; y <= maxY + 1; y++)
            {
                for (var x = minX - 1; x <= maxX + 1; x++)
                {
                    var pointStr = "";
                    for (var y2 = -1; y2 <= 1; y2++)
                    {
                        for (var x2 = -1; x2 <= 1; x2++)
                        {
                            var px = x + x2;
                            var py = y + y2;
                            if (px < minX || px > maxX || py < minY || py > maxY)
                            {
                                pointStr += outsideState;
                            }
                            else
                            {
                                pointStr += points.Contains(new Point(px, py)) ? "1" : "0";
                            }
                        }
                    }

                    var pos = Convert.ToInt32(pointStr, 2);
                    var charAt = alg[pos];
                    if (charAt == '#')
                    {
                        newPoints.Add(new Point(x, y));
                    }
                }
            }

            return newPoints;
        }

        private static List<Point> ParsePoints(string[] lines)
        {
            var  points = new List<Point>();

            var minX = 0;
            var minY = 0;

            for (int i = 2, y = minY; i < lines.Length; i++, y++)
            {
                var line = lines[i];
                for (int j = 0, x = minX; j < line.Length; j++, x++)
                {
                    if (line[j] == '#')
                    {
                        points.Add(new Point(x, y));
                    }
                }
            }

            return points;
        }

        private static void WritePoints(List<Point> points, string state, StringBuilder builder = null)
        {
            var minX = points.Min(p => p.X);
            var maxX = points.Max(p => p.X);
            var minY = points.Min(p => p.Y);
            var maxY = points.Max(p => p.Y);

            Action<string> output = builder != null ? (s) => builder.Append(s) : Console.Write;

            for (var y = Math.Min(minY - 5, -5); y <= Math.Max(maxY + 5, 5); y++)
            {
                for (var x = Math.Min(minX - 5, -5); x <= Math.Max(maxX + 5, 5); x++)
                {
                    if (x < minX || x > maxX || y < minY || y > maxY)
                    {
                        output(state == "1" ? "#" : ".");
                    } 
                    else if (points.Contains(new Point(x, y)))
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

    public record Point(int X, int Y);
}