using common;
using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace day2
{
    public class Navigating_Submarine
    {
        [Test]
        public void Yields_Product_Of_Position_And_Depth()
        {
            var instructions = GetInstructions();

            var location = new Submarine(Submarine.SimpleStrategies).ExecuteInstructions(instructions);

            Assert.AreEqual(1660158, location.pos * location.depth);
        }

        [Test]
        public void Yields_Product_Of_Position_And_Depth_From_Aim()
        {
            var instructions = GetInstructions();

            var location = new Submarine(Submarine.ComplexInstructions).ExecuteInstructions(instructions);

            Assert.AreEqual(1604592846, location.pos * location.depth);
        }

        public static (string instruction, int length)[] GetInstructions()
        {
            return Resources.GetResourceLines(typeof(Navigating_Submarine), "day2.input.txt").Select(x => {
                var parts = x.Split(' ');
                var instruction = (instruction:parts[0], length:Convert.ToInt32(parts[1]));
                return instruction;
            }).ToArray();
        }
    }
}