using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using common;
using NUnit.Framework;
using static System.Convert;

namespace day5
{
    public class Hypothermal_Vents
    {
        [TestCase("day5.sample.txt", 5, true)]
        [TestCase("day5.input.txt", 7436, false, Category = "Slow")]
        public void Are_Best_Avoided_When_Overlapping(string input, int expected, bool draw)
        {
            var inputData = Resources.GetResourceLines(GetType(), input);
            var allSegments = Parse(inputData);
            var segments = allSegments.Where(x => x.IsStraight).ToArray();

            var overlaps = CountOverlaps(draw, segments);

            Assert.AreEqual(expected, overlaps);
        }

        [TestCase("day5.sample.txt", 12, true)]
        [TestCase("day5.input.txt", 21104, true, Category = "Slow")]
        public void Are_Best_Avoided_When_Overlapping_Including_Diagonal(string input, int expected, bool draw)
        {
            var inputData = Resources.GetResourceLines(GetType(), input);
            var allSegments = Parse(inputData);
            
            var overlaps = CountOverlaps(draw, allSegments.ToArray());

            Assert.AreEqual(expected, overlaps);
        }

        private static int CountOverlaps(bool draw, Line[] segments)
        {
            var overlaps = 0;

            var area = (
                X1: segments.Min(x => x.MinX),
                Y1: segments.Min(x => x.MinY),
                X2: segments.Max(x => x.MaxX),
                Y2: segments.Max(x => x.MaxY)
            );

            for (var y = area.Y1; y <= area.Y2; y++)
            {
                for (var x = area.X1; x <= area.X2; x++)
                {
                    var p = new Point(x, y);
                    var intersecting = segments.Where(x => x.Intersects(p));
                    var count = intersecting.Count();

                    if (count > 1)
                    {
                        overlaps++;
                    }

                    if (draw)
                    {
                        if (count > 0)
                        {
                            Console.Write(count);
                        }
                        else
                        {
                            Console.Write('.');
                        }
                    }
                }

                if (draw)
                {
                    Console.Write(Environment.NewLine);
                }
            }

            return overlaps;
        }

        [TestCase(9, 7, 7, 9, 8, 8, true)]
        [TestCase(7, 9, 9, 7, 8, 8, true)]
        [TestCase(2, 5, 5, 8, 3, 6, true)]
        [TestCase(2, 5, 5, 8, 4, 6, false)]
        [TestCase(0, 0, 0, -10, 0, -5, true)]
        [TestCase(0, 0, -10, 0, -3, 0, true)]
        [TestCase(0, 0, 10, 0, 3, 0, true)]
        [TestCase(0, 0, 10, 0, -3, 0, false)]
        public void Can_Intersect_Diagonally(int x1, int y1, int x2, int y2, int px, int py, bool expected)
        {
            var xy1 = new Point(x1, y1);
            var xy2 = new Point(x2, y2);
            var line = new Line(xy1, xy2);

            Assert.AreEqual(expected, line.Intersects(new Point(px, py)));
        }

        private Area Parse(string[] inputData)
        {
            return new Area(inputData.Select(ParseLine));
        }

        private Line ParseLine(string arg)
        {
            var parts = arg.Split(" -> ");
            var xy1 = ParsePoint(parts[0]);
            var xy2 = ParsePoint(parts[1]);
            return new Line(xy1, xy2);
        }

        private Point ParsePoint(string part)
        {
            var parts = part.Split(',');
            return new Point(ToInt32(parts[0]), ToInt32(parts[1]));
        }

        class Area : List<Line>
        {
            public Area()
            {
            }

            public Area(IEnumerable<Line> collection) : base(collection)
            {
            }
        }

        record Point(int X, int Y);

        record Line
        {
            public readonly Point From;
            public readonly Point To;

            public readonly bool IsStraight;
            public readonly int MinX;
            public readonly int MaxX;
            public readonly int MinY;
            public readonly int MaxY;

            public readonly int Slope;
            public readonly int YIntercept;

            public Line(Point from, Point to)
            {
                From = from;
                To = to;

                IsStraight = From.X == To.X || From.Y == To.Y;
                MinX = Math.Min(From.X, To.X);
                MaxX = Math.Max(From.X, To.X);
                MinY = Math.Min(From.Y, To.Y);
                MaxY = Math.Max(From.Y, To.Y);

                if (To.X - From.X != 0)
                { 
                    Slope = (To.Y - From.Y) / (To.X - From.X);
                    YIntercept = (From.X * Slope - From.Y) * -1;
                }
            }

            public bool Intersects(Point point)
            {
                var withinY = point.Y >= MinY && point.Y <= MaxY;
                if (To.X - From.X == 0)
                {
                    return point.X == From.X && withinY;
                }

                var withinX = point.X >= MinX && point.X <= MaxX;
                if (To.Y - From.Y == 0)
                {
                    return point.Y == From.Y && withinX;
                }

                return (point.X * Slope + YIntercept) == point.Y
                    && withinX && withinY;
            }
        };
    }
}