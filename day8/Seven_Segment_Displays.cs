using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalTests.Reporters.TestFrameworks;
using common;
using Newtonsoft.Json;
using NUnit.Framework;
using static System.String;

namespace day8
{
    [TestFixture]
    public class Seven_Segment_Displays
    {
        [TestCase("day8.simplesample.txt", 0)]
        [TestCase("day8.sample.txt", 26)]
        [TestCase("day8.input.txt", 476)]
        public void Have_Four_Unique_Patterns(string resource, int expectedSimpleOccurrences)
        {
            var entries = Entry.ParseEntries(resource);

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
            var entries = Entry.ParseEntries(resource);

            long total = 0;
            foreach (var entry in entries)
            {
                var connector = new DisplayConnector(entry);
                var display = connector.ConnectDisplay();

                total += display.Number;

                if (draw)
                {
                    Console.WriteLine();
                    display.ListPatterns();
                    display.DrawDigitConnections();
                    display.ShowOrderedConnections();
                    Console.WriteLine();
                }

                Console.WriteLine(display);
            }

            Console.WriteLine($"Total {total}");

            Assert.AreEqual(expectedTotal, total);
        }

        [Test]
        [TestCase("day8.simplesample.txt")]
        [UseReporter(typeof(VisualStudioReporter))]
        public void Swaps_Segment_By_Segment_When_Visualized(string resource)
        {
            var entries = Entry.ParseEntries(resource);

            var builder = new StringBuilder();

            foreach (var entry in entries)
            {
                builder.AppendLine(Join(" ", entry.Digits.Select(x => x.PatternString)));
                builder.AppendLine();

                var connector = new DisplayConnector(entry);
                using var enumerator = connector.ConnectSlowly().GetEnumerator();
               
                while(enumerator.MoveNext())
                {
                    DrawDigits(entry.Digits, enumerator.Current, x => builder.Append(x));
                }
            }

            Console.WriteLine(builder.ToString());
            
            Approvals.Verify(builder.ToString());
        }

        public static void DrawDigits(Pattern[] digits, Connections connections, Action<string> draw = null, string padding = "  ")
        {
            draw ??= Console.Write;
            var (a, b, c, d, e, f, g) = connections;

            draw($" {a}{a}{a}{a} {padding+padding}");

            for (var col = 0; col < 4; col++)
            {
                draw(digits[col].Contains(a) ? $" {a}{a}{a}{a} " : $"      ");
                draw(padding);
            }
            draw(Environment.NewLine);

            for (var i = 0; i < 2; i++)
            {
                draw($"{b}    {c}{padding + padding}");

                for (var col = 0; col < 4; col++)
                {
                    draw(digits[col].Contains(b) ? $"{b}    " : $"     ");
                    draw(digits[col].Contains(c) ? $"{c}" : $" ");
                    draw(padding);
                }

                draw(Environment.NewLine);
            }

            draw($" {d}{d}{d}{d} {padding + padding}");
            for (var col = 0; col < 4; col++)
            {
                draw(digits[col].Contains(d) ? $" {d}{d}{d}{d} " : $"      ");
                draw(padding);
            }

            draw(Environment.NewLine);

            for (var i = 0; i < 2; i++)
            {
                draw($"{e}    {f}{padding + padding}");

                for (var col = 0; col < 4; col++)
                {
                    draw(digits[col].Contains(e) ? $"{e}    " : $"     ");
                    draw(digits[col].Contains(f) ? $"{f}" : $" ");
                    draw(padding);
                }

                draw(Environment.NewLine);
            }

            draw($" {g}{g}{g}{g} {padding + padding}");
            for (var col = 0; col < 4; col++)
            {
                draw(digits[col].Contains(g) ? $" {g}{g}{g}{g} " : $"      ");
                draw(padding);
            }

            draw(Environment.NewLine);
            draw(Environment.NewLine);
        }
    }

    public record Display(
        Pattern[] OrderedPatterns,
        Connections Connections,
        Pattern[] Digits
    )
    {
        readonly Dictionary<string, int> values = new()
        {
            { OrderedPatterns[0].Key, 0 },
            { OrderedPatterns[1].Key, 1 },
            { OrderedPatterns[2].Key, 2 },
            { OrderedPatterns[3].Key, 3 },
            { OrderedPatterns[4].Key, 4 },
            { OrderedPatterns[5].Key, 5 },
            { OrderedPatterns[6].Key, 6 },
            { OrderedPatterns[7].Key, 7 },
            { OrderedPatterns[8].Key, 8 },
            { OrderedPatterns[9].Key, 9 },
        };

        public int Digit1 => values[Digits[0].Key];
        public int Digit2 => values[Digits[1].Key];
        public int Digit3 => values[Digits[2].Key];
        public int Digit4 => values[Digits[3].Key];
        public int Number => Digit1 * 1000 + Digit2 * 100 + Digit3 * 10 + Digit4;

        public override string ToString() => $"{Digits[0].PatternString} {Digits[1].PatternString} {Digits[2].PatternString} {Digits[3].PatternString} = {Number}";

        public void ShowOrderedConnections()
        {
            var connections = Connections;
            var (a, b, c, d, e, f, g) = connections;
            Console.WriteLine($"a: {a}");
            Console.WriteLine($"b: {b}");
            Console.WriteLine($"c: {c}");
            Console.WriteLine($"d: {d}");
            Console.WriteLine($"d: {e}");
            Console.WriteLine($"f: {f}");
            Console.WriteLine($"f: {g}");
        }

        public void DrawDigitConnections()
        {
            DrawDigitConnections(Connections);
        }

        public static void DrawDigitConnections(Connections connections, Action<string> draw = null)
        {
            draw ??= Console.WriteLine;
            var (a, b, c, d, e, f, g) = connections;
            draw($" {a}{a}{a}{a} ");
            draw($"{b}    {c}");
            draw($"{b}    {c}");
            draw($" {d}{d}{d}{d} ");
            draw($"{e}    {f}");
            draw($"{e}    {f}");
            draw($" {g}{g}{g}{g} ");
            draw("");
        }

        public void ListPatterns()
        {
            for (var i = 0; i < 10; i++)
            {
                Console.WriteLine($"{i}: {OrderedPatterns[i]}");
            }
            Console.WriteLine();
        }
    }

    public readonly struct Connections
    {
        public readonly char a;
        public readonly char b;
        public readonly char c;
        public readonly char d;
        public readonly char e;
        public readonly char f;
        public readonly char g;

        public Connections(char a, char b, char c, char d, char e, char f, char g)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.e = e;
            this.f = f;
            this.g = g;
        }

        public void Deconstruct(out char a, out char b, out char c, out char d, out char e, out char f, out char g)
        {
            a = this.a;
            b = this.b;
            c = this.c;
            d = this.d;
            e = this.e;
            f = this.f;
            g = this.g;
        }

        public static implicit operator (char a, char b, char c, char d, char e, char f, char g)(Connections c) => (c.a, c.b, c.c, c.d, c.e, c.f, c.g);
        public static implicit operator Connections((char a, char b, char c, char d, char e, char f, char g) c) => new(c.a, c.b, c.c, c.d, c.e, c.f, c.g);
    }

    public class DisplayConnector
    {
        private bool created = false;
        private bool enumerating = false;

        private Entry entry;
        private Pattern[] sorted;
        private Dictionary<char, char> dc;
        private Pattern[] fiveSegs;
        private Pattern[] sixSegs;
        private Pattern[] sixNine;
        private char a;
        private char b;
        private char c;
        private char d;
        private char e;
        private char f;
        private char g;
        private Display display;
        private Connections connections;

        public DisplayConnector(Entry entry)
        {
            this.entry = entry;
            sorted = new Pattern[10];
            dc = new[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g' }.ToDictionary(x => x);
            connections = Swap(dc, 'a', 'a');
        }

        public Display Display => display;

        public Connections Connections => connections;

        public IEnumerable<Connections> ConnectSlowly()
        {
            if (created || enumerating)
            {
                throw new Exception("Already connected. Create another connector.");
            }
            enumerating = true;

            yield return connections;
            connections = ConnectToA();
            yield return connections;
            connections = ConnectToF();
            yield return connections;
            connections = ConnectToC();
            yield return connections;
            connections = ConnectToD();
            yield return connections;
            connections = ConnectToB();
            yield return connections;
            connections = ConnectToE();
            yield return connections;
            connections = ConnectToG();
            yield return connections;

            display = new Display(sorted, (a, b, c, d, e, f, g), entry.Digits);
        }

        public Display ConnectDisplay(Action<Pattern[], Connections> draw = null)
        {
            if (created || enumerating)
            {
                throw new Exception("Already connected. Create another connector.");
            }

            created = true;

            connections = ConnectToA();
            draw?.Invoke(entry.Digits, connections);

            connections = ConnectToF();
            draw?.Invoke(entry.Digits, connections);

            connections = ConnectToC();
            draw?.Invoke(entry.Digits, connections);

            connections = ConnectToD();
            draw?.Invoke(entry.Digits, connections);

            connections = ConnectToB();
            draw?.Invoke(entry.Digits, connections);

            connections = ConnectToE();
            draw?.Invoke(entry.Digits, connections);

            connections = ConnectToG();
            draw?.Invoke(entry.Digits, connections);

            display = new Display(sorted, (a, b, c, d, e, f, g), entry.Digits);

            return display;
        }

        private Connections ConnectToA()
        {
            sorted[1] = entry.SingleByLength(2);
            sorted[7] = entry.SingleByLength(3);
            sorted[4] = entry.SingleByLength(4);
            sorted[8] = entry.SingleByLength(7);
            fiveSegs = entry.FindByLength(5);
            sixSegs = entry.FindByLength(6);

            a = sorted[7].SubtractUnique(sorted[1]);
            return Swap(dc, 'a', a);
        }

        private Connections ConnectToF()
        {
            f = entry.MostCommonSegment();
            return Swap(dc, 'f', f);
        }

        private Connections ConnectToC()
        {
            c = sorted[7].SubtractUnique(a, f);

            return Swap(dc, 'c', c);
        }

        private Connections ConnectToD()
        {
            sorted[3] = sorted[7].MostSimilar(fiveSegs);

            d = (sorted[3] + sorted[4]).SubtractUnique(c, f);

            return Swap(dc, 'd', d);
        }

        private Connections ConnectToB()
        {
            sorted[0] = sixSegs.Single(x => x.DoesNotHave(d));

            sixNine = sixSegs.Except(sorted[0]).ToArray();

            sorted[9] = sorted[7].MostSimilar(sixNine);
            sorted[6] = sixNine.Except(sorted[9]).Single();

            b = sorted[4].SubtractUnique(sorted[3]);

            return Swap(dc, 'b', b);
        }

        private Connections ConnectToE()
        {
            var twoFive = fiveSegs.Except(sorted[3]).ToArray();
            sorted[5] = twoFive.Single(x => x.Contains(f));
            sorted[2] = twoFive.Except(sorted[5]).Single();

            e = sorted[2].SubtractUnique(sorted[3]);

            return Swap(dc, 'e', e);
        }

        private Connections ConnectToG()
        {
            g = new[] {'a', 'b', 'c', 'd', 'e', 'f', 'g'}.SubtractUnique(a, b, c, d, e, f);

            return Swap(dc, 'g', g);
        }

        private static Connections Swap(Dictionary<char, char> dc, char logical, char actual)
        {
            var fromKey = dc.First(x => x.Value == actual).Key;
            var oldVal = dc[logical];
            dc[fromKey] = oldVal;
            dc[logical] = actual;
            return (a: dc['a'], b: dc['b'], c: dc['c'], d: dc['d'], e: dc['e'], f: dc['f'], g: dc['g']);
        }
    }

    public record Entry(Pattern[] Patterns, Pattern[] Digits)
    {
        public Pattern SingleByLength(int length) => Patterns.Single(x => x.Length == length);

        public Pattern[] FindByLength(int length) => Patterns.Where(x => x.Length == length).ToArray();

        public char MostCommonSegment() => Patterns.SelectMany(x => x.Segments).GroupBy(x => x).OrderByDescending(x => x.Count()).First().Key;

        public static IEnumerable<Entry> ParseEntries(string resource)
        {
            var lines = Resources.GetResourceLines(typeof(Seven_Segment_Displays), resource);
            var entries = lines.Select(line =>
            {
                var parts = line.Split(" | ");
                var patterns = parts[0].Split(' ').Select(x => new Pattern(x.Trim())).ToArray();
                var digits = parts[1].Split(' ').Select(x => new Pattern(x.Trim())).ToArray();
                return new Entry(patterns, digits);
            });
            return entries;
        }
    }

    public record Pattern(string PatternString) : IEnumerable<char>
    {
        public readonly char[] Segments = PatternString.OrderBy(x => x).ToArray();
        public readonly string Key = Join("", PatternString.OrderBy(x => x)); // CreateKey(PatternString);

        public readonly int[] InitialCandidates = PatternString.Length switch
        {
            2 => new[] { 1 },
            3 => new[] { 7 },
            4 => new[] { 4 },
            5 => new[] { 2, 3, 5 },
            6 => new[] { 0, 6, 9 },
            7 => new[] { 8 },
            _ => throw new Exception($"Incompatible segment string '{PatternString}' ({PatternString.Length}) {String.Join(",", PatternString.ToCharArray())}")
        };

        public bool IsUnique => InitialCandidates.Length == 1;

        public int Length => Segments.Length;

        public int CommonSegmentCount(Pattern other) => this.Intersect(other).Count();

        public Pattern MostSimilar(Pattern[] segments) => segments.Select(x => (x, count: x.CommonSegmentCount(this))).OrderByDescending(x => x.count).First().x;

        public bool DoesNotHave(char d)
        {
            return Segments.All(y => y != d);
        }

        public static implicit operator char[](Pattern pattern) => pattern.Segments;

        public static IEnumerable<char> operator +(Pattern x, Pattern y) => x.Intersect(y);

        public override string ToString() => PatternString;

        public IEnumerator<char> GetEnumerator() => Segments.AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public static class PatternExtension
    {
        public static char SubtractUnique(this IEnumerable<char> segments, params char[] other) => segments.Except(other).Single();

        public static IEnumerable<Pattern> Except(this IEnumerable<Pattern> patterns, Pattern other) => patterns.Except(new[] { other });
    }
}