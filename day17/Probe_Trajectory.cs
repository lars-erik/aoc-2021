using System;
using System.Linq;
using common;
using NUnit.Framework;

namespace day17
{
    public class Probe_Trajectory
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase("target area: x=20..30, y=-10..-5")]
        // P1 is 23 102
        public void Goes_Sky_High(string input)
        {
            var parts = input.Split(' ');
            var xPart = parts[2].Trim(',');
            var yPart = parts[3];
            var xArea = xPart.Split('=')[1].Split("..");
            var yArea = yPart.Split('=')[1].Split("..");

            Console.WriteLine(xArea[0] + " - " + xArea[1]);
            Console.WriteLine(yArea[0] + " - " + yArea[1]);

            var xLengths = Enumerable.Range(1, 300)
                .ToDictionary(x => x * (x + 1) / 2, x => x);

            xLengths.Dump();

            Assert.Fail();
        }
    }
}