using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;

namespace day2
{
    public class Navigating_Submarine
    {
        [Test]
        public void Yields_Product_Of_Position_And_Depth()
        {
            var input = new StreamReader(GetType().Assembly.GetManifestResourceStream("day2.input.txt")).ReadToEnd();
            var instructions = input.Split(Environment.NewLine).Select(x => {
                var parts = x.Split(' ');
                var instruction = (instruction:parts[0], length:Convert.ToInt32(parts[1]));
                return instruction;
            }).ToArray();

            var pos = 0;
            var depth = 0;

            var ops = new Dictionary<string, Action<int>> {
                { "forward", x => pos += x },
                { "up", x => depth -= x },
                { "down", x => depth += x }
            };

            foreach(var instr in instructions)
            {
                ops[instr.instruction](instr.length);
            }

            Assert.AreEqual(1660158, pos * depth);
        }
    }
}