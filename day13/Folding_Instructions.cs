using System;
using System.Collections.Generic;
using System.Linq;
using common;
using Newtonsoft.Json;
using NUnit.Framework;

namespace day13
{
    [TestFixture]
    public class Folding_Instructions
    {
        private List<Point> points;
        private List<(string axis, int coord)> instructions;
        private int maxX;
        private int maxY;

        [SetUp]
        public void Setup()
        {
            var lines = Resources.GetResourceLines(typeof(Folding_Instructions), TestContext.CurrentContext.Test.Arguments[0]!.ToString());
            points = new List<Point>();
            instructions = new List<(string axis, int coord)>();
            int i;
            for (i = 0; i < lines.Length && lines[i] != ""; i++)
            {
                var parts = lines[i].Split(',');
                var point = new Point(Convert.ToInt32(parts[0]), Convert.ToInt32(parts[1]));
                points.Add(point);
            }

            maxX = points.Max(p => p.X);
            maxY= points.Max(p => p.Y);

            for (i++; i < lines.Length; i++)
            {
                var parts = lines[i].Substring(11).Split('=');
                instructions.Add((parts[0], Convert.ToInt32(parts[1])));
            }
        }

        [Test]
        [TestCase("day13.sample.txt", 17)]
        [TestCase("day13.input.txt", 671)]
        public void Yields_New_Picture(string resource, int expectedAfterFirst)
        {
            Console.WriteLine(JsonConvert.SerializeObject(points));
            Console.WriteLine(JsonConvert.SerializeObject(instructions));

            List<Point> newPoints = null;
            for (var i = 0; i < 1; i++) // instructions.Count
            {
                var instr = instructions[i];
                newPoints = new List<Point>();
                foreach (var p in points)
                {
                    Point newPoint = p;
                    if (instr.axis == "y")
                    {
                        if (p.Y > instr.coord)
                        {
                            newPoint = new Point(p.X, maxY - p.Y);
                        }
                    }
                    else // axis x
                    {
                        if (p.X > instr.coord)
                        { 
                            newPoint = new Point(maxX - p.X, p.Y);
                        }
                    }
                    newPoints.Add(newPoint);

                }

                points = newPoints;
                maxX = points.Max(x => x.X);
                maxY = points.Max(x => x.Y);
            }

            maxX = points.Max(x => x.X);
            maxY = points.Max(x => x.Y);
            int dots = 0;

            for (var y = 0; y <= maxY; y++)
            {
                for (var x = 0; x <= maxX; x++)
                {
                    if (newPoints.Contains(new(x, y)))
                    {
                        Console.Write("#");
                        dots++;
                    }
                    else
                    {
                        Console.Write(".");
                    }
                }
                Console.WriteLine();
            }

            Assert.AreEqual(expectedAfterFirst, dots);
        }

        [Test]
        [TestCase("day13.sample.txt")]
        [TestCase("day13.input.txt")]
        public void Yields_New_Full_Picture(string resource)
        {
            //Console.WriteLine(JsonConvert.SerializeObject(points));
            //Console.WriteLine(JsonConvert.SerializeObject(instructions));

            Console.WriteLine($"{maxX}, {maxY}");

            List<Point> newPoints = null;
            for (var i = 0; i < instructions.Count; i++)
            {
                var instr = instructions[i];
                Console.WriteLine(instr.axis + "=" + instr.coord);
                newPoints = new List<Point>();
                foreach (var p in points)
                {
                    Point newPoint = p;
                    if (instr.axis == "y")
                    {
                        if (p.Y > instr.coord)
                        {
                            newPoint = new Point(p.X, instr.coord - (p.Y - instr.coord));
                        }
                    }
                    else // axis x
                    {
                        if (p.X > instr.coord)
                        { 
                            newPoint = new Point(instr.coord - (p.X - instr.coord), p.Y);
                        }
                    }
                    newPoints.Add(newPoint);
                }

                points = newPoints;
                if (instr.axis == "y")
                {
                    maxY = instr.coord;
                }
                else
                { 
                    maxX = instr.coord;
                }
                Console.WriteLine($"{maxX}, {maxY}");

            }

            Console.WriteLine();

            int dots = 0;

            for (var y = 0; y <= maxY; y++)
            {
                for (var x = 0; x <= maxX; x++)
                {
                    if (newPoints.Contains(new(x, y)))
                    {
                        Console.Write("#");
                        dots++;
                    }
                    else
                    {
                        Console.Write(".");
                    }
                }
                Console.WriteLine();
            }

        }

        record Point(int X, int Y)
        {
            public override string ToString()
            {
                return $"{X},{Y}";
            }
        };
    }
}