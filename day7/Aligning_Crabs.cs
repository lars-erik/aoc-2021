using System;
using System.Linq;
using common;
using NUnit.Framework;

namespace day7
{
    public class Aligning_Crabs
    {
        [TestCase("day7.sample.txt", 2, 37)]
        [TestCase("day7.input.txt", 361, 364898)]
        public void Consumes_Least_Fuel_At_Given_Position(string resource, int expectedPosition, decimal expectedFuel)
        {
            var positions = Resources.GetSeparatedIntegers(GetType(), resource);

            var minPos = positions.Min();
            var maxPos = positions.Max();

            Console.WriteLine($"min: {minPos}, max: {maxPos}, count: {positions.Length}");

            var curBestPos = -1;
            var curBestFuel = decimal.MaxValue;

            for (var testedPos = minPos; testedPos <= maxPos; testedPos++)
            {
                var curFuel = 0m;
                for (var position = 0; position < positions.Length; position++)
                {
                    var dist = Math.Abs(positions[position] - testedPos);
                    curFuel += dist;
                }

                if (curFuel < curBestFuel)
                {
                    curBestPos = testedPos;
                    curBestFuel = curFuel;
                }
            }

            Console.WriteLine($"Found best position {curBestPos} at {curBestFuel} fuel.");

            Assert.AreEqual(expectedPosition, curBestPos);
            Assert.AreEqual(expectedFuel, curBestFuel);
        }
    }
}