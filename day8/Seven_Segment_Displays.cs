using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using common;
using Newtonsoft.Json;
using NUnit.Framework;

namespace day8
{
    public class Seven_Segment_Displays
    {
        [TestCase("day8.simplesample.txt", 0)]
        [TestCase("day8.sample.txt", 26)]
        [TestCase("day8.input.txt", 476)]
        public void Are_Wired_As_Bad_As_Led_Matrixes(string resource, int expectedSimpleOccurrences)
        {
            var entries = ParseEntries(resource);

            var totalSimples = 0;
            foreach (var entry in entries)
            {
                var simples = entry.Digits.Where(x => entry.GetPattern(x).IsUnique);
                totalSimples += simples.Count();
            }

            Assert.AreEqual(expectedSimpleOccurrences, totalSimples);
        }

        private static IEnumerable<Entry> ParseEntries(string resource)
        {
            var lines = Resources.GetResourceLines(typeof(Seven_Segment_Displays), resource);
            var entries = lines.Select(line =>
            {
                var parts = line.Split(" | ");
                var patterns = parts[0].Split(' ').Select(x => new Pattern(x)).ToArray();
                var digits = parts[1].Split(' ');
                return new Entry(patterns, digits);
            });
            return entries;
        }

        record Entry(Pattern[] Patterns, string[] Digits)
        {
            private readonly Dictionary<string, Pattern> byKey = Patterns.ToDictionary(x => x.Key);

            public Pattern GetPattern(string segments)
            {
                return byKey[Pattern.CreateKey(segments)];
            }
        }

        record Pattern(string Segments)
        {
            public readonly string Key = CreateKey(Segments);

            public readonly int[] InitialCandidates = Segments.Length switch
            {
                2 => new[] {1},
                3 => new[] {7},
                4 => new[] {4},
                5 => new[] {2, 3, 5},
                6 => new[] {0, 6, 9},
                7 => new[] {8},
                _ => throw new Exception("Incompatible segment string")
            };

            public bool IsUnique => InitialCandidates.Length == 1;

            public static string CreateKey(string segments)
            {
                return String.Join("", segments.OrderBy(x => x));
            }
        }
    }
}