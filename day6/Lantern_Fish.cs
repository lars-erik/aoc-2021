using System;
using System.Collections.Generic;
using System.Linq;
using common;
using Newtonsoft.Json;
using NUnit.Framework;

namespace day6
{
    public class Lantern_Fish
    {
        [TestCase("day6.sample.txt", 5934, 80)]
        [TestCase("day6.input.txt", 372984, 80)]
        [TestCase("day6.sample.txt", 26984457539, 256)]
        [TestCase("day6.input.txt", 1681503251694, 256)]
        public void Multiply_Exponentially(string resource, long expected, int daysToRun)
        {
            var fish = GetInitialFish(resource);

            for (var d = 0; d < daysToRun; d++)
            {
                fish = Multiply(fish);
            }

            var totalFish = fish.Sum(x => (long)x.Value);
            fish.Add(99, totalFish);
            Console.Write(JsonConvert.SerializeObject(fish, Formatting.Indented));
            Assert.AreEqual(expected, totalFish);
        }

        public static Dictionary<int, long> Multiply(Dictionary<int, long> fish)
        {
            fish = new Dictionary<int, long>
            {
                {0, fish[1]},
                {1, fish[2]},
                {2, fish[3]},
                {3, fish[4]},
                {4, fish[5]},
                {5, fish[6]},
                {6, fish[7] + fish[0]},
                {7, fish[8]},
                {8, fish[0]},
            };
            return fish;
        }

        public static Dictionary<int, long> GetInitialFish(string resource)
        {
            var fish = Enumerable.Range(0, 9).ToDictionary(x => x, x => (long) 0);
            var inputFish = Resources.GetSeparatedIntegers(typeof(Lantern_Fish), resource)
                .GroupBy(x => x)
                .ToDictionary(x => x.Key, x => x.Count());

            foreach (var key in fish.Keys)
            {
                if (inputFish.TryGetValue(key, out int val))
                {
                    fish[key] = val;
                }
            }

            return fish;
        }
    }
}