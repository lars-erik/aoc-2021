using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using ApprovalTests;
using ApprovalTests.Core;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using common;
using NUnit.Framework;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace day9
{
    [TestFixture]
    public class Smoke_Flow_Modelling
    {
        private static readonly (int x, int y)[] Checks = new[]
        {
            (x: -1, y: 0),
            (x: 0, y: -1),
            (x: 1, y: 0),
            (x: 0, y: 1),
        };

        private int areaLength;
        private int areaWidth;
        private int[][] matrix;
        private bool[][] stateMatrix;
        private int[][] basinMatrix;
        private Dictionary<int, (int x, int y)> bedPoints;

        [SetUp]
        public void Setup()
        {
            var lines = Resources.GetResourceLines(typeof(Smoke_Flow_Modelling), TestContext.CurrentContext.Test.Arguments[0]!.ToString());
            matrix = lines.Select(x => x.ToCharArray().Select(x => int.Parse(x.ToString())).ToArray()).ToArray();

            areaLength = matrix.Length;
            areaWidth = matrix[0].Length;
            stateMatrix = new bool[areaLength][];
            for (var i = 0; i < areaLength; i++) stateMatrix[i] = new bool[areaWidth];
            basinMatrix = new int[areaLength][];
            for (var i = 0; i < areaLength; i++) basinMatrix[i] = new int[areaWidth];
            bedPoints = new Dictionary<int, (int x, int y)>();
        }

        [Test]
        [TestCase("day9.sample.txt", 15)]
        [TestCase("day9.input.txt", 570)]
        [UseReporter(typeof(ShellLauncherReporter))]
        public void Calculates_Risk_Score_For_Low_Points(string resource, int expectedRisk)
        {
            IterateMatrix(FindBasinBeds);

            var totalRisk = 0;
            IterateMatrix(p =>
            {
                if (stateMatrix[p.y][p.x])
                {
                    totalRisk += 1 + matrix[p.y][p.x];
                }
            });

            Assert.AreEqual(expectedRisk, totalRisk);

            var image = DrawBasinBeds();
            var bytes = image.ToArray();

            using (ApprovalResults.ForScenario(resource))
            {
                Approvals.VerifyBinaryFile(bytes, ".png");
            }
        }

        [Test]
        [TestCase("day9.sample.txt", 1134)]
        [TestCase("day9.input.txt", 899392)]
        [UseReporter(typeof(ShellLauncherReporter))]
        public void Finds_The_Three_Largest_Basins_To_Avoid(string resource, int expectedSize)
        {
            IterateMatrix(FindBasinBeds);

            Dictionary<int, int> basinSizes = new Dictionary<int, int>();

            var currentBasinId = 1;
            IterateMatrix(p =>
            {
                if (stateMatrix[p.y][p.x])
                {
                    basinMatrix[p.y][p.x] = currentBasinId;
                    basinSizes.Add(currentBasinId, 1);
                    bedPoints.Add(currentBasinId, p);

                    Stack<(int x, int y)> candidates = new Stack<(int x, int y)>();
                    ForNeighbours(p, (p, n) => candidates.Push(n));
                    while (candidates.Count > 0)
                    {
                        var cp = candidates.Pop();
                        var extBasinId = basinMatrix[cp.y][cp.x];
                        var height = matrix[cp.y][cp.x];
                        if (extBasinId == 0 && height < 9)
                        {
                            basinMatrix[cp.y][cp.x] = currentBasinId;
                            basinSizes[currentBasinId]++;
                            ForNeighbours(cp, (p, n) =>
                            {
                                if (basinMatrix[n.y][n.x] == 0 && matrix[n.y][n.x] < 9)
                                { 
                                    candidates.Push(n);
                                }
                            });
                        }
                    }

                    currentBasinId++;
                }
            });

            var largestThreeSizes = basinSizes
                .Select(x => x.Value)
                .OrderByDescending(x => x)
                .Take(3)
                .Aggregate(1, (prev, cur) => prev * cur);

            Assert.AreEqual(expectedSize, largestThreeSizes);

            DrawAsciiBasins();


            using var stream = DrawDepthColoredBasins(basinSizes);

            var bytes = stream.ToArray();

            using (ApprovalResults.ForScenario(resource))
            {
                Approvals.VerifyBinaryFile(bytes, ".png");
            }
        }

        private MemoryStream DrawDepthColoredBasins(Dictionary<int, int> basinSizes)
        {
            var maxId = basinSizes.Keys.Max();
            var colorRatio = 360f / maxId;
            var random = new Random(1337);

            var randomizedIds = new Stack<int>(basinSizes.Keys.OrderBy(x => random.Next()));
            var colorIdMap = new Dictionary<int, int>();
            foreach (var key in basinSizes.Keys)
            {
                colorIdMap.Add(key, randomizedIds.Pop());
            }

            var pixelSize = 1920 / areaWidth;

            using var img = new SixLabors.ImageSharp.Image<Rgba32>(areaWidth * pixelSize, areaLength * pixelSize);
            var spaceConverter = new ColorSpaceConverter();

            IterateMatrix(p =>
            {
                var imgX = p.x * pixelSize;
                var imgY = p.y * pixelSize;

                Rgba32 color;

                var basinId = basinMatrix[p.y][p.x];
                if (basinId > 0)
                {
                    var hue = colorIdMap[basinId] * colorRatio;
                    var height = matrix[p.y][p.x];
                    var saturation = height / 8f;
                    var easedSat = (float)Math.Pow(saturation, 3);
                    Hsl idColor = new Hsl(hue, .7f, .2f + easedSat * .6f);
                    color = spaceConverter.ToRgb(idColor);
                }
                else
                {
                    var hue = AverageColorOfNeighbourhood(p, colorIdMap, colorRatio);

                    color = spaceConverter.ToRgb(new Hsl(hue, .4f, .8f));
                }

                for (var y2 = imgY; y2 < imgY + pixelSize; y2++)
                {
                    for (var x2 = imgX; x2 < imgX + pixelSize; x2++)
                    {
                        img[x2, y2] = color;
                    }
                }
            });

            img.Mutate(x => x.BokehBlur(24, 2, 1));
            img.Mutate(x => x.GaussianSharpen(4));

            var coll = new FontCollection();
            var family = coll.Install(@"c:\windows\fonts\OCRAEXT.TTF");
            var font = family.CreateFont(15, FontStyle.Regular);
            foreach (var bedId in bedPoints.Keys)
            {
                var p = bedPoints[bedId];
                var imgX = p.x * pixelSize + pixelSize / 2;
                var imgY = p.y * pixelSize + pixelSize / 2;

                var text = bedId.ToString();
                var measure = TextMeasurer.Measure(text, new RendererOptions(font));
                img.Mutate(x => x.DrawText(text, font, Color.White, new PointF(imgX - measure.Width / 2, imgY - measure.Height / 2 - 2)));
                text = (9 - matrix[p.y][p.x]) + " m";
                measure = TextMeasurer.Measure(text, new RendererOptions(font));
                img.Mutate(x => x.DrawText(text, font, Color.White, new PointF(imgX - measure.Width / 2, imgY + measure.Height / 2 - 2)));
            }

            using var stream = new MemoryStream();
            img.Save(stream, new PngEncoder());
            return stream;
        }

        private float AverageColorOfNeighbourhood((int x, int y) p, Dictionary<int, int> colorIdMap, float colorRatio)
        {
            var ids = new List<int>();

            void CollectNeighbourColors((int x, int y) np, int level)
            {
                var nId = basinMatrix[np.y][np.x];
                if (nId > 0)
                {
                    ids.Add(nId);
                }
                else if (level < 5)
                {
                    ForNeighbours(np, (p, n) => { CollectNeighbourColors(n, level + 1); });
                }
            }

            ForNeighbours(p, (p, n) => { CollectNeighbourColors(n, 0); });
            var colors = ids.Select(x => colorIdMap[x] * colorRatio);
            var hue = colors.Average();
            return hue;
        }

        private void DrawAsciiBasins()
        {
            for (var y = 0; y < areaLength; y++)
            {
                for (var x = 0; x < areaWidth; x++)
                {
                    var basinId = basinMatrix[y][x];
                    if (basinId > 0)
                    {
                        Console.Write(basinId.ToString(" 000"));
                    }
                    else
                    {
                        Console.Write(" ...");
                    }
                }

                Console.WriteLine();
            }
        }

        private MemoryStream DrawBasinBeds()
        {
            var pixelSize = 5;
            using var img = new SixLabors.ImageSharp.Image<Rgba32>(areaWidth * pixelSize, areaLength * pixelSize);

            IterateMatrix(p =>
            {
                var imgX = p.x * pixelSize;
                var imgY = p.y * pixelSize;
                Rgba32 color;

                if (stateMatrix[p.y][p.x])
                {
                    color = new Rgba32(0, 0, 128, 255);
                }
                else
                {
                    color = new Rgba32(0, 128, 255, 255);
                }

                for (var y2 = imgY; y2 < imgY + pixelSize; y2++)
                {
                    for (var x2 = imgX; x2 < imgX + pixelSize; x2++)
                    {
                        img[x2, y2] = color;
                    }
                }
            });

            using var stream = new MemoryStream();
            img.Save(stream, new PngEncoder());
            return stream;
        }

        private void IterateMatrix(Action<(int x, int y)> action)
        {

            for (var y = 0; y < areaLength; y++)
            {
                for (var x = 0; x < areaWidth; x++)
                {
                    action((x, y));
                }
            }
        }

        private void FindBasinBeds((int x, int y) point)
        {
            var thisHeight = matrix[point.y][point.x];
            var thisIsLowest = true;

            ForNeighbours(point, (p, other) =>
            {
                var otherHeight = matrix[other.y][other.x];
                if (otherHeight <= thisHeight)
                {
                    thisIsLowest = false;
                }
            });
            stateMatrix[point.y][point.x] = thisIsLowest;
        }

        private void ForNeighbours((int x, int y) point, Action<(int x, int y), (int x, int y)> action)
        {
            foreach (var check in Checks)
            {
                var otherPoint = (x: point.x + check.x, y: point.y + check.y);
                if (otherPoint.x >= 0 && otherPoint.x < areaWidth && otherPoint.y >= 0 && otherPoint.y < areaLength)
                {
                    action(point, otherPoint);
                }
            }
        }
    }

    public class ShellLauncherReporter : IApprovalFailureReporter
    {
        public static readonly ShellLauncherReporter INSTANCE = new ShellLauncherReporter();

        public void Report(string approved, string received)
        {
            QuietReporter.DisplayCommandLineApproval(approved, received);
            var startInfo = new ProcessStartInfo(received)
            {
                UseShellExecute = true
            };
            Process.Start(startInfo);
        }
    }
}