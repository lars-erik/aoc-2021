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

            Assert.AreEqual(expected, overlaps);
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

        record Line(Point From, Point To)
        {
            public bool IsStraight => From.X == To.X || From.Y == To.Y;
            public int MinX => Math.Min(From.X, To.X);
            public int MaxX => Math.Max(From.X, To.X);
            public int MinY => Math.Min(From.Y, To.Y);
            public int MaxY => Math.Max(From.Y, To.Y);

            public bool Intersects(Point point)
            {
                return (point.X >= MinX && point.X <= MaxX && (point.Y == MinY || point.Y == MaxY)) 
                    || (point.Y >= MinY && point.Y <= MaxY && (point.X == MinX || point.X == MaxX));
            }
        };
    }
}