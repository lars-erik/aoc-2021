using System;
using System.Collections.Generic;
using System.Linq;
using common;
using NUnit.Framework;

namespace day17
{
    public class Probe_Trajectory
    {
        private int maxY;
        private List<(int x, int y)> velocities;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase("target area: x=20..30, y=-10..-5", 45, 112)]
        [TestCase("target area: x=265..287, y=-103..-58", 5253, 1770)]
        public void Goes_Sky_High(string input, int expectedHeight, int expectedVectors)
        {
            var parts = input.Split(' ');
            var xPart = parts[2].Trim(',');
            var yPart = parts[3];
            var xArea = xPart.Split('=')[1].Split("..")
                .Select(int.Parse)
                .OrderBy(x => x)
                .ToArray();
            var yArea = yPart.Split('=')[1].Split("..")
                .Select(int.Parse)
                .OrderByDescending(x => x)
                .ToArray();

            Console.WriteLine(xArea[0] + " - " + xArea[1]);
            Console.WriteLine(yArea[0] + " - " + yArea[1]);

            var xLengths = Enumerable.Range(1, 300)
                .ToDictionary(x => x * (x + 1) / 2, x => x);

            var startAtX = xLengths.First(x => x.Key >= xArea[0]).Value;
            var endAtX = xArea[1];

            var startAtY = yArea[1];
            var endAtY = Math.Abs(yArea[1]);

            var maxSteps = 1000;

            Console.WriteLine("Start at x {0}, end at x {1}", startAtX, endAtX);
            Console.WriteLine("Start at y {0}, end at y {1}", startAtY, endAtY);

            velocities = new List<(int x, int y)>();
            maxY = 0;
            var maxVec = (x: 0, y: 0);

            for (var x = startAtX; x <= endAtX; x++)
            {
                for (var y = startAtY; y <= endAtY; y++)
                {
                    var curMaxY = 0;

                    var point = (x: 0, y: 0);
                    var velocity = (x, y);
                    var step = 0;
                    while (step < maxSteps)
                    {
                        point = (x: point.x + velocity.x, y: point.y + velocity.y);
                        var adjX = velocity.x >= 0 ? -1 : 1;
                        velocity = (x: Math.Max(velocity.x + adjX, 0), y: velocity.y - 1);

                        if (point.y > curMaxY)
                        {
                            curMaxY = point.y;
                        }

                        // TODO: Negative X
                        var withinX = point.x >= xArea[0] && point.x <= xArea[1];
                        var withinY = point.y <= yArea[0] && point.y >= yArea[1];

                        if (withinX && withinY)
                        {
                            velocities.Add((x, y));

                            if (curMaxY > maxY)
                            {
                                maxY = curMaxY;
                                maxVec = (x, y);
                            }

                            break;
                        }

                        if (point.x > xArea[1] || point.y < yArea[1])
                        {
                            // Overshot
                            break;
                        }

                        step++;
                    }
                }
            }

            Console.WriteLine("Highest point at {0} with vec {1}, {2}", maxY, maxVec.x, maxVec.y);

            Console.WriteLine(velocities.Count);
            foreach(var vel in velocities)
                Console.WriteLine(vel.x + ", " + vel.y);

            Assert.AreEqual(expectedHeight, maxY);
            Assert.AreEqual(expectedVectors, velocities.Count);
        }
    }
}