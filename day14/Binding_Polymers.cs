using System;
using System.Collections.Generic;
using System.Linq;
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
        [TestCase("day14.sample.txt", true, 1588)]
        [TestCase("day14.input.txt", false, 3831)]
        public void Puts_Elements_Between_Letters(string resource, bool print, int expected)
        {
            for(var step = 0; step < 10; step++)
            { 
                var nextInput = input.Substring(0, 1);
                for (var i = 0; i < input.Length - 1; i++)
                {
                    var pair = input.Substring(i, 2);
                    var inBetween = pairs[pair];
                    nextInput += inBetween + pair[1];
                }

                input = nextInput;
                
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
    }
}