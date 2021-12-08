using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using common;
using Newtonsoft.Json;
using NUnit.Framework;
using static System.String;

namespace day8
{
    public class Seven_Segment_Displays
    {
        [TestCase("day8.simplesample.txt", 0)]
        [TestCase("day8.sample.txt", 26)]
        [TestCase("day8.input.txt", 476)]
        public void Have_Four_Unique_Segments(string resource, int expectedSimpleOccurrences)
        {
            var entries = ParseEntries(resource);

            var totalSimples = 0;
            foreach (var entry in entries)
            {
                var simples = entry.Digits.Where(x => x.IsUnique);
                totalSimples += simples.Count();
            }

            Assert.AreEqual(expectedSimpleOccurrences, totalSimples);
        }

        [TestCase("day8.simplesample.txt", 5353, true)]
        [TestCase("day8.sample.txt", 61229, false)]
        [TestCase("day8.input.txt", 1011823, false)]
        public void Are_Wired_As_Bad_As_Led_Matrixes(string resource, int expectedTotal, bool draw)
        {
            var entries = ParseEntries(resource);

            long total = 0;
            foreach (var entry in entries)
            {
                var patterns = entry.Patterns.Select(x => x.Segments).ToArray();

                var pattern1 = patterns.Single(x => x.Length == 2);
                var pattern7 = patterns.Single(x => x.Length == 3);
                var pattern4 = patterns.Single(x => x.Length == 4);
                var pattern8 = patterns.Single(x => x.Length == 7);
                var fiveSegs = patterns.Where(x => x.Length == 5).ToArray();
                var sixSegs = patterns.Where(x => x.Length == 6).ToArray();

                var a = pattern7.Except(pattern1).Single();
                var f = patterns.SelectMany(x => x).GroupBy(x => x).OrderByDescending(x => x.Count()).First().Key;
                var c = pattern7.Except(new[] {a, f}).Single();

                var pattern3 = fiveSegs.Single(x => x.Intersect(pattern7).Count() == 3);

                var d = pattern3.Intersect(pattern4).Except(new[] {c, f}).Single();

                var pattern0 = sixSegs.Single(x => x.All(y => y != d));
                
                var sixNine = sixSegs.Where(x => !x.SequenceEqual(pattern0)).ToArray();

                var pattern9 = sixNine.Single(x => x.Intersect(pattern7).Count() == 3);
                var pattern6 = sixNine.Single(x => x != pattern9);

                var b = pattern4.Except(pattern3).Single();

                var twoFive = fiveSegs.Where(x => !x.SequenceEqual(pattern3)).ToArray();
                var pattern5 = twoFive.Single(x => x.Contains(f));
                var pattern2 = twoFive.Except(new[]{pattern5}).Single();

                var e = pattern2.Except(pattern3).Single();
                var g = new[] {'a', 'b', 'c', 'd', 'e', 'f', 'g'}.Except(new[] {a, b, c, d, e, f}).Single();

                var values = new Dictionary<string, int>()
                {
                    {Join("", pattern0), 0},
                    {Join("", pattern1), 1},
                    {Join("", pattern2), 2},
                    {Join("", pattern3), 3},
                    {Join("", pattern4), 4},
                    {Join("", pattern5), 5},
                    {Join("", pattern6), 6},
                    {Join("", pattern7), 7},
                    {Join("", pattern8), 8},
                    {Join("", pattern9), 9},
                };

                var digit1 = values[entry.Digits[0].Key];
                var digit2 = values[entry.Digits[1].Key];
                var digit3 = values[entry.Digits[2].Key];
                var digit4 = values[entry.Digits[3].Key];

                var result = digit1 * 1000 + digit2 * 100 + digit3 * 10 + digit4;
                total += result;

                Console.WriteLine($"{entry.Digits[0].PatternString} {entry.Digits[1].PatternString} {entry.Digits[2].PatternString} {entry.Digits[3].PatternString} = {result}");

                if (draw)
                { 
                    Console.WriteLine();
                    ListPatterns(pattern0, pattern1, pattern2, pattern3, pattern4, pattern5, pattern6, pattern7, pattern8, pattern9);
                    DrawDigitConnections(a, b, c, d, e, f, g);
                    ShowOrderedConnections(a, b, c, d, e, f, g);
                }
            }

            Console.WriteLine($"Total {total}");

            Assert.AreEqual(expectedTotal, total);
        }

        private static void ShowOrderedConnections(char a, char b, char c, char d, char e, char f, char g)
        {
            Console.WriteLine($"a: {a}");
            Console.WriteLine($"b: {b}");
            Console.WriteLine($"c: {c}");
            Console.WriteLine($"d: {d}");
            Console.WriteLine($"d: {e}");
            Console.WriteLine($"f: {f}");
            Console.WriteLine($"f: {g}");
        }

        private static void DrawDigitConnections(char a, char b, char c, char d, char e, char f, char g)
        {
            Console.WriteLine($" {a}{a}{a}{a} ");
            Console.WriteLine($"{b}    {c}");
            Console.WriteLine($"{b}    {c}");
            Console.WriteLine($" {d}{d}{d}{d} ");
            Console.WriteLine($"{e}    {f}");
            Console.WriteLine($"{e}    {f}");
            Console.WriteLine($" {g}{g}{g}{g} ");
            Console.WriteLine();
        }

        private static void ListPatterns(char[] pattern0, char[] pattern1, char[] pattern2, char[] pattern3, char[] pattern4,
            char[] pattern5, char[] pattern6, char[] pattern7, char[] pattern8, char[] pattern9)
        {
            Console.WriteLine($"0: {Join("", pattern0)}");
            Console.WriteLine($"1: {Join("", pattern1)}");
            Console.WriteLine($"2: {Join("", pattern2)}");
            Console.WriteLine($"3: {Join("", pattern3)}");
            Console.WriteLine($"4: {Join("", pattern4)}");
            Console.WriteLine($"5: {Join("", pattern5)}");
            Console.WriteLine($"6: {Join("", pattern6)}");
            Console.WriteLine($"7: {Join("", pattern7)}");
            Console.WriteLine($"8: {Join("", pattern8)}");
            Console.WriteLine($"9: {Join("", pattern9)}");
            Console.WriteLine();
        }

        private static IEnumerable<Entry> ParseEntries(string resource)
        {
            var lines = Resources.GetResourceLines(typeof(Seven_Segment_Displays), resource);
            var entries = lines.Select(line =>
            {
                var parts = line.Split(" | ");
                var patterns = parts[0].Split(' ').Select(x => new Pattern(x)).ToArray();
                var digits = parts[1].Split(' ').Select(x => new Pattern(x)).ToArray();
                return new Entry(patterns, digits);
            });
            return entries;
        }

        record Entry(Pattern[] Patterns, Pattern[] Digits)
        {

            public Pattern FindUniquePattern(int digit)
            {
                return Patterns.Single(x => x.InitialCandidates.All(y => y == digit));
            }
        }

        record Pattern(string PatternString)
        {
            public readonly char[] Segments = PatternString.OrderBy(x => x).ToArray();
            public readonly string Key = Join("", PatternString.OrderBy(x => x)); // CreateKey(PatternString);

            public readonly int[] InitialCandidates = PatternString.Length switch
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
        }
    }
}