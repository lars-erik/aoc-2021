using System;
using System.Linq;
using common;
using NUnit.Framework;

namespace day6
{
    public class Lantern_Fish
    {
        [TestCase("day6.sample.txt", 5934, 80)]
        [TestCase("day6.input.txt", 372984, 80)]
        [TestCase("day6.sample.txt", 5934, 256, Category = "Slow")]
        public void Multiply_Exponentially(string resource, int expected, int daysToRun)
        {
            var lines = Resources.GetResourceLines(typeof(Lantern_Fish), resource);
            var data = lines[0];
            var fish = data.Split(',').Select(x => Convert.ToInt16(x)).ToList();

            for (var d = 0; d < daysToRun; d++)
            {
                var fishCount = fish.Count;
                for (var f = 0; f < fishCount; f++)
                {
                    var it = fish[f];
                    fish[f]--;
                    if (fish[f] == -1)
                    {
                        fish[f] = 6;
                        fish.Add(8);
                    }
                }
            }

            Assert.AreEqual(expected, fish.Count);
        }
    }
}