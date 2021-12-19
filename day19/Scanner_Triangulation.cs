using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using common;
using NUnit.Framework;

namespace day19
{
    public class Scanner_Triangualation
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Solves_2D_Sample()
        {
            const string input = @"--- scanner 0 ---
0,2,0
4,1,0
3,3,0

--- scanner 1 ---
-1,-1,0
-5,0,0
-2,1,0";
            var scanners = Parse(input);

            const int treshold = 3;

            var distances = CalculateAllDistances(scanners);

            var equalProbes = FindEqualProbes(distances, treshold);

            ListEqualProbes(equalProbes);

            Assert.That(equalProbes[scanners[0]][scanners[1]].Values.Count == 3);
            Assert.That(equalProbes[scanners[1]][scanners[0]].Values.Count == 3);
        }

        [Test]
        [TestCase("day19.sample.txt")]
        [TestCase("day19.input.txt")]
        public void Solves_Sample(string resource)
        {
            var input = Resources.GetResourceLines(typeof(Scanner_Triangualation), resource);
            var scanners = Parse(input);

            var distances = CalculateAllDistances(scanners);

            var equalProbes = FindEqualProbes(distances, 12);

            //ListEqualProbes(equalProbes);

            var positioned = new List<Scanner> {scanners[0]};
            var unpositioned = new Queue<Scanner>(scanners.Except(new []{scanners[0]}));
            while (unpositioned.Count > 0)
            {
                var scanner = unpositioned.Dequeue();
                var adjacent = equalProbes[scanner];
                var scannerO = positioned.FirstOrDefault(ps => adjacent.Keys.Contains(ps) && adjacent[ps].Values.Any());
                if (scannerO == null)
                {
                    unpositioned.Enqueue(scanner);
                    continue;
                }

                var wasPositioned = false;

                foreach(var rotationSet in RotationMatrixes)
                {
                    var rotation = rotationSet.matrix;
                    //Console.WriteLine("Matrix: " + rotationSet.index + " - " + RotationCandidates[rotationSet.index]);

                    var firstProbe = adjacent[scannerO].First();

                    var firstTp = firstProbe.Key.Location;
                    //Console.WriteLine("First point: " + firstProbe.Value.AdjustedLocation + " - " + firstTp);
                    firstTp = firstTp.Rotate(rotation);
                    //Console.WriteLine("First point rotated: " + firstTp);
                    var offset = firstProbe.Value.AdjustedLocation - firstTp;
                    //Console.WriteLine("Offset: " + offset);

                    var pointsComparison = adjacent[scannerO]
                        .Select(pair =>
                        {
                            // TODO: We have to try 24 directions and compare all points here?
                            var originProbe = pair.Value.AdjustedLocation;
                            var tp = pair.Key.Location;

                            tp = tp.Rotate(rotation);

                            var adjusted = tp + offset;

                            var newDiff = originProbe - adjusted;

                            //Console.WriteLine(adjusted + " (" + newDiff.Length() + ")");

                            return (diff:newDiff, adjusted);
                        })
                        .ToArray();

                    var distinctDiffs = pointsComparison.Select(x => x.diff).Distinct().Count();

                    //Console.WriteLine();
                    if (distinctDiffs > 1)
                    {
                        //Console.WriteLine("Coords doesn't match up");
                    }
                    else
                    {
                        scanner.SetRotation(rotation);
                        scanner.SetOffset(offset);
                        positioned.Add(scanner);
                        wasPositioned = true;

                        Console.WriteLine("Well waddya know");
                        break;
                    }
                    //Console.WriteLine();
                }

                if (!wasPositioned)
                {
                    unpositioned.Enqueue(scanner);
                }
            }

            var unique = scanners.SelectMany(x => x.Probes.Select(y => y.AdjustedLocation.ToString())).Distinct().ToList();
            unique.Dump();

            unique.Count.Dump();

            Assert.Fail();
        }

        private static void ListEqualProbes(Dictionary<Scanner, Dictionary<Scanner, Dictionary<Probe, Probe>>> equalProbes)
        {

            foreach (var scannerAPair in equalProbes)
            {
                foreach (var scannerBPair in scannerAPair.Value)
                {
                    foreach (var probePair in scannerBPair.Value)
                    {
                        Console.WriteLine($"{scannerAPair.Key.Id}: {probePair.Key.Location}, {scannerBPair.Key.Id}: {probePair.Value.Location}");
                    }
                }
            }
        }

        private static Dictionary<Scanner, Dictionary<Scanner, Dictionary<Probe, Probe>>> FindEqualProbes((Scanner s, Dictionary<Probe, Dictionary<Probe, double>> distances)[] distances, int treshold)
        {
            var equalProbes = new Dictionary<Scanner, Dictionary<Scanner, Dictionary<Probe, Probe>>>();
            var otherTreshold = treshold - 1;
            for (var i = 0; i < distances.Length; i++)
            {
                var scannerA = distances[i];
                var scannerProbes = new Dictionary<Scanner, Dictionary<Probe, Probe>>();
                equalProbes.Add(scannerA.s, scannerProbes);

                for (var j = 0; j < distances.Length; j++)
                {
                    if (i == j) continue;

                    var scannerB = distances[j];
                    var equalScannerProbes = new Dictionary<Probe, Probe>();
                    scannerProbes.Add(scannerB.s, equalScannerProbes);

                    foreach (var probeASet in scannerA.distances)
                    {
                        foreach (var probeBSet in scannerB.distances)
                        {
                            var aValues = probeASet.Value.Values;
                            var bValues = probeBSet.Value.Values;
                            var equal = aValues.Intersect(bValues).Count();
                            if (equal >= otherTreshold)
                            {
                                equalScannerProbes.Add(probeASet.Key, probeBSet.Key);
                            }
                        }
                    }
                }
            }

            return equalProbes;
        }

        private static (Scanner s, Dictionary<Probe, Dictionary<Probe, double>> distances)[] CalculateAllDistances(List<Scanner> scanners)
        {
            var distances = scanners.Select(x => (s: x, distances: x.CalculateDistances())).ToArray();
            return distances;
        }

        private static List<Scanner> Parse(string input)
        {
            var lines = input.Replace("\r", "").Split("\n");
            return Parse(lines);
        }

        private static List<Scanner> Parse(string[] lines)
        {
            var scanners = new List<Scanner>();
            foreach (var line in lines)
            {
                if (line.StartsWith("---"))
                {
                    scanners.Add(new Scanner(scanners.Count));
                }
                else if (!String.IsNullOrEmpty(line))
                {
                    var points = line.Split(',').Select(float.Parse).ToArray();
                    scanners.Last().Add(new Vector3(points[0], points[1], points[2]));
                }
            }

            return scanners;
        }

        private static readonly float PI = (float)Math.PI;
        private static readonly float HalfPI = (float)Math.PI / 2;
        private static readonly float NegHalfPI = (float)Math.PI / -2;
        private static readonly Vector4[] RotationCandidates = {
            new(0, 0, 0, 0),
            new(0, 0, HalfPI, 0),
            new(0, 0, PI, 0),
            new(0, 0, NegHalfPI, 0),
            new(HalfPI, 0, 0, 0),
            new(HalfPI, 0, HalfPI, 0),
            new(HalfPI, 0, PI, 0),
            new(HalfPI, 0, NegHalfPI, 0),
            new(PI, 0, 0, 0),
            new(PI, 0, HalfPI, 0),
            new(PI, 0, PI, 0),
            new(PI, 0, NegHalfPI, 0),
            new(NegHalfPI, 0, 0, 0),
            new(NegHalfPI, 0, HalfPI, 0),
            new(NegHalfPI, 0, PI, 0),
            new(NegHalfPI, 0, NegHalfPI, 0),
            new(0, HalfPI, 0, 0),
            new(0, HalfPI, HalfPI, 0),
            new(0, HalfPI, PI, 0),
            new(0, HalfPI, NegHalfPI, 0),
            new(0, NegHalfPI, 0, 0),
            new(0, NegHalfPI, HalfPI, 0),
            new(0, NegHalfPI, PI, 0),
            new(0, NegHalfPI, NegHalfPI, 0)
        };

        private static readonly (int index, Matrix4x4 matrix)[] RotationMatrixes = RotationCandidates.Select((v, i) => 
            (
                index:i,
                matrix:
                    new Matrix4x4(1, 0, 0, 0, 0, MathF.Cos(v.X), -MathF.Sin(v.X), 0, 0, MathF.Sin(v.X), MathF.Cos(v.X), 0, 0, 0, 0, 0) *
                    new Matrix4x4(MathF.Cos(v.Y), 0, MathF.Sin(v.Y), 0, 0, 1, 0, 0, -MathF.Sin(v.Y), 0, MathF.Cos(v.Y), 0, 0, 0, 0, 0) *
                    new Matrix4x4(MathF.Cos(v.Z), -MathF.Sin(v.Z), 0, 0, MathF.Sin(v.Z), MathF.Cos(v.Z), 0, 0, 0, 0, 1, 0, 0, 0, 0, 0)
            )
        ).ToArray();
    }

    public class Scanner
    {
        private List<Probe> probes = new List<Probe>();
        private Matrix4x4 rotation = Matrix4x4.Identity;
        private Vector3 offset = Vector3.Zero;

        public int Id { get; }
        public List<Probe> Probes => probes;

        public Matrix4x4 Rotation => rotation;

        public Vector3 Offset => offset;

        public Scanner(int id)
        {
            Id = id;
        }

        public void Add(Vector3 probeLocation)
        {
            probes.Add(new Probe(this, probeLocation));
        }

        public Dictionary<Probe, Dictionary<Probe, double>> CalculateDistances()
        {
            var set = new Dictionary<Probe, Dictionary<Probe, double>>();
            foreach (var probe in Probes)
            {
                set.Add(probe, probe.CalculateDistances());
            }

            return set;
        }

        public void SetRotation(Matrix4x4 rotation)
        {
            this.rotation = rotation;
        }

        public void SetOffset(Vector3 offset)
        {
            this.offset = offset;
        }
    }

    public class Probe
    {
        private readonly Scanner scanner;
        public Vector3 Location { get; }

        public Vector3 AdjustedLocation => Location.Rotate(scanner.Rotation) + scanner.Offset; 

        public Probe(Scanner scanner, Vector3 location)
        {
            this.scanner = scanner;
            Location = location;
        }

        public Dictionary<Probe, double> CalculateDistances()
        {
            var set = new Dictionary<Probe, double>();
            foreach (var probeB in scanner.Probes.Except(new []{this}))
            {
                var dist = Math.Sqrt(
                    Math.Pow(probeB.Location.X-Location.X, 2) +
                    Math.Pow(probeB.Location.Y-Location.Y, 2) +
                    Math.Pow(probeB.Location.Z-Location.Z, 2)
                );
                set.Add(probeB, dist);
            }

            return set;
        }

        public override string ToString()
        {
            return Location.ToString();
        }
    }

    public static class VectorExtensions
    {
        public static Vector3 Rotate(this Vector3 tp, Matrix4x4 rotation)
        {
            return new Vector3(
                MathF.Round(tp.X * rotation.M11 + tp.Y * rotation.M12 + tp.Z * rotation.M13),
                MathF.Round(tp.X * rotation.M21 + tp.Y * rotation.M22 + tp.Z * rotation.M23),
                MathF.Round(tp.X * rotation.M31 + tp.Y * rotation.M32 + tp.Z * rotation.M33)
            );
        }
    }
}