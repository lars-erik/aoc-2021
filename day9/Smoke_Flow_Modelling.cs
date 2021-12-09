using System;
using System.Linq;
using common;
using NUnit.Framework;

namespace day9
{
    public class Smoke_Flow_Modelling
    {
        [TestCase("day9.sample.txt", 15)]
        [TestCase("day9.input.txt", 570)]
        public void Calculates_Risk_Score_For_Low_Points(string resource, int expectedRisk)
        {
            var lines = Resources.GetResourceLines(typeof(Smoke_Flow_Modelling), resource);
            var matrix = lines.Select(x => x.ToCharArray().Select(x => int.Parse(x.ToString())).ToArray()).ToArray();
            
            var areaLength = matrix.Length;
            var areaWidth = matrix[0].Length;

            var stateMatrix = new bool[areaLength][];
            for (var i = 0; i < areaLength; i++) stateMatrix[i] = new bool[areaWidth];

            var checks = new[]
            {
                (x: -1, y: 0),
                (x: 0, y: -1),
                (x: 1, y: 0),
                (x: 0, y: 1),
            };

            var totalRisk = 0;
            for (var y = 0; y < areaLength; y++)
            {
                for (var x = 0; x < areaWidth; x++)
                {
                    var thisHeight = matrix[y][x];
                    var thisIsLowest = true;
                    foreach (var check in checks)
                    {
                        var x2 = x + check.x;
                        var y2 = y + check.y;
                        if (x2 >= 0 && x2 < areaWidth && y2 >= 0 && y2 < areaLength)
                        {
                            var otherHeight = matrix[y2][x2];
                            if (otherHeight <= thisHeight)
                            {
                                thisIsLowest = false;
                            }
                        }
                    }

                    stateMatrix[y][x] = thisIsLowest;
                    if (thisIsLowest)
                    {
                        totalRisk += 1 + thisHeight;
                    }
                }
            }

            for (var y = 0; y < areaLength; y++)
            {
                for (var x = 0; x < areaWidth; x++)
                {
                    if (stateMatrix[y][x])
                        Console.Write($"*{matrix[y][x]}* ");
                    else
                        Console.Write($" {matrix[y][x]}  ");
                }
                Console.WriteLine();
            }

            Assert.AreEqual(expectedRisk, totalRisk);
        }
    }
}