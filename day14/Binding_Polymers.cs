using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using common;
using Newtonsoft.Json;
using NUnit.Framework;

namespace day14
{
    public class Binding_Polymers
    {
        private Dictionary<string, string> pairs;
        private string input;

        [SetUp]
        public void Setup()
        {
            var lines = Resources.GetResourceLines(typeof(Binding_Polymers), TestContext.CurrentContext.Test.Arguments[0]!.ToString());
            input = lines[0];
            pairs = new Dictionary<string, string>();
            for (var line = 2; line < lines.Length; line++)
            {
                var pair = lines[line].Split(" -> ");
                pairs.Add(pair[0], pair[1]);
            }
        }

        [Test]
        [TestCase("day14.sample.txt", true, 1588, 10)]
        [TestCase("day14.input.txt", false, 3831, 10)]
        public void Puts_Elements_Between_Letters(string resource, bool print, int expected, int maxSteps)
        {
            for(var step = 0; step < maxSteps; step++)
            { 
                var nextInput = new StringBuilder(input.Length * 2);
                nextInput.Append(input.Substring(0, 1));
                for (var i = 0; i < input.Length - 1; i++)
                {
                    var pair = input.Substring(i, 2);
                    var inBetween = pairs[pair];
                    nextInput.Append(inBetween + pair[1]);
                }

                input = nextInput.ToString();
                
                if(print)
                { 
                    Console.WriteLine($"{step + 1}: {input.Length} {input}");
                }
            }

            var elements = input
                .ToCharArray()
                .GroupBy(x => x)
                .Select(x => new {c = x.Key, count = x.Count()})
                .OrderByDescending(x => x.count)
                .ToArray();
            var mostOccurring = elements.First();
            var leastOccurring = elements.Last();
            var result = mostOccurring.count - leastOccurring.count;

            Assert.AreEqual(expected, result);
        }

        [Test]
        [TestCase("day14.sample.txt", true, 2188189693529, 40)]
        [TestCase("day14.input.txt", true, 5725739914282, 40)]
        //[TestCase("day14.input.txt", false, 3831, 10)]
        public void Manages_Huge_Polymers(string resource, bool print, long expected, int maxSteps)
        {
            var pairOccs = new Dictionary<string, long>();
            for (var i = 0; i < input.Length - 1; i++)
            {
                var pair = input.Substring(i, 2);
                if (!pairOccs.ContainsKey(pair)) pairOccs.Add(pair, 0);
                pairOccs[pair]++;
            }

            pairOccs.Dump();

            for (var step = 0; step < maxSteps; step++)
            {
                var newPairOccs = new Dictionary<string, long>();

                foreach (var key in pairOccs.Keys)
                {
                    var insert = pairs[key];
                    var newLeft = key[0] + insert;
                    var newRight = insert + key[1];

                    if (!newPairOccs.ContainsKey(newLeft)) newPairOccs.Add(newLeft, 0);
                    if (!newPairOccs.ContainsKey(newRight)) newPairOccs.Add(newRight, 0);

                    newPairOccs[newLeft] += pairOccs[key];
                    newPairOccs[newRight] += pairOccs[key];
                }

                pairOccs = newPairOccs;
                newPairOccs.Dump();
            }

            var charOccs = new Dictionary<char, long>
            {
                { input[0], 1 }
            };
            foreach (var key in pairOccs.Keys)
            {
                var b = key[1];
                if (!charOccs.ContainsKey(b)) charOccs.Add(b, 0);
                charOccs[b] += pairOccs[key];
            }

            var max = charOccs.Values.Max();
            var min = charOccs.Values.Min();

            charOccs.Dump();

            Assert.AreEqual(expected, max - min);

        }
    }
}