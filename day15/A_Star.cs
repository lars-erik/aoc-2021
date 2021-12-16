using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using common;
using NUnit.Framework;

namespace day15
{
    public class A_Star
    {
        private int[,] matrix;
        private List<(int x, int y)> neighbours = new List<(int x, int y)>
        {
            (-1, 0),
            (0, -1),
            (1, 0),
            (0, 1)
        };
        private int maxY;
        private int maxX;
        private List<Point> openSet;
        private int[,] gScore;
        private int[,] fScore;
        private Point[,] cameFrom;

        [SetUp]
        public void Setup()
        {
            var lines = Resources.GetResourceLines(typeof(A_Star), TestContext.CurrentContext.Test.Arguments[0]!.ToString());
            matrix = new int[lines.Length, lines[0].Length];
            maxY = matrix.GetUpperBound(0);
            maxX = matrix.GetUpperBound(1);
            openSet = new List<Point>();
            cameFrom = new Point[maxY + 1, maxX + 1];
            gScore = new int[maxY + 1, maxX + 1];
            fScore = new int[maxY + 1, maxX + 1];

            Iterate((x, y) => matrix[y, x] = Convert.ToInt32(lines[y][x].ToString()));
            Iterate((x, y) => gScore[y, x] = fScore[y, x] = Int16.MaxValue);
        }

        private void Iterate(Action<int, int> action)
        {
            for (var y = 0; y <= maxY; y++)
            {
                for (var x = 0; x <= maxX; x++)
                {
                    action(x, y);
                }
            }
        }

        [Test]
        [TestCase("day15.sample.txt", 40)]
        [TestCase("day15.input.txt", 423)]
        public void Finds_The_Least_Risky_Path(string resource, int expectedTotalRisk)
        {
            WriteMatrix();

            openSet.Add(new(0, 0, matrix[0, 0]));
            gScore[0, 0] = 0;
            fScore[0, 0] = 0;

            var bestPath = FindBestPaths();

            Console.WriteLine(bestPath.Total);
            Console.WriteLine(String.Join(" > ", bestPath.Points.Select(x => x.ToString())));
            

            Assert.AreEqual(expectedTotalRisk, bestPath.Total);
        }

        [Test]
        [TestCase("day15.sample.txt", 315)]
        [TestCase("day15.input.txt", 2778)]
        public void Finds_The_Least_Risky_Path_In_Full_Map(string resource, int expectedTotalRisk)
        {

            var newMatrix = new int[(maxY + 1) * 5, (maxX + 1) * 5];
            for (var ty = 0; ty < 5; ty++)
            {
                for (var tx = 0; tx < 5; tx++)
                {
                    Iterate((x, y) =>
                    {
                        var copyToX = x + tx * (maxX + 1);
                        var copyToY = y + ty * (maxY + 1);
                        newMatrix[copyToY, copyToX] = (matrix[y, x] + ty + tx);
                        if (newMatrix[copyToY, copyToX] > 9)
                            newMatrix[copyToY, copyToX] -= 9;
                    });
                }
            }

            matrix = newMatrix;

            maxY = matrix.GetUpperBound(0);
            maxX = matrix.GetUpperBound(1);
            cameFrom = new Point[maxY + 1, maxX + 1];
            gScore = new int[maxY + 1, maxX + 1];
            fScore = new int[maxY + 1, maxX + 1];

            Iterate((x, y) => gScore[y, x] = fScore[y, x] = Int16.MaxValue);

            openSet.Add(new(0, 0, matrix[0, 0]));
            gScore[0, 0] = 0;
            fScore[0, 0] = 0;

            var bestPath = FindBestPaths();

            Console.WriteLine(bestPath.Total);
            //Console.WriteLine(String.Join(" > ", bestPath.Points.Select(x => x.ToString())));

            Assert.AreEqual(expectedTotalRisk, bestPath.Total);
        }

        private Path FindBestPaths()
        {
            while(openSet.Any())
            {
                var current = openSet.OrderBy(p => fScore[p.Y, p.X]).First();
                var isGoal = current.X == maxX && current.Y == maxY;

                if (isGoal)
                {
                    var path = new Path();
                    while (!(current.X == 0 && current.Y == 0))
                    {
                        path.Points.Insert(0, current);
                        current = cameFrom[current.Y, current.X];
                    }
                    path.Points.Insert(0, current);
                    return path;
                }

                openSet.Remove(current);

                var theseNeighbours = NeighboursByRisk(current);

                for (var i = 0; i < theseNeighbours.Count; i++)
                {
                    Point candidate = theseNeighbours[i];

                    var tentativeScore = gScore[current.Y, current.X] + matrix[candidate.Y, candidate.X];

                    if (tentativeScore < gScore[candidate.Y, candidate.X])
                    {
                        cameFrom[candidate.Y, candidate.X] = current;
                        gScore[candidate.Y, candidate.X] = tentativeScore;
                        fScore[candidate.Y, candidate.X] = tentativeScore;

                        if (!openSet.Contains(candidate))
                        {
                            openSet.Add(candidate);
                        }
                    }
                }
            }

            throw new Exception("Couldn't find best path");
        }

        private List<Point> NeighboursByRisk(Point pt)
        {
            return neighbours.Select(rel =>
                {
                    var x = pt.X + rel.x;
                    var y = pt.Y + rel.y;

                    if (y >= 0 && y <= maxY && x >= 0 && x <= maxX)
                    {
                        return new Point(x, y, matrix[y, x]);
                    }

                    return new Point(-1, -1, -1);
                })
                .Where(p => p.X > -1)
                .ToList();
        }

        private void WriteMatrix()
        {
            for (var y = 0; y <= matrix.GetUpperBound(0); y++)
            {
                for (var x = 0; x <= matrix.GetUpperBound(1); x++)
                {
                    Console.Write(matrix[y, x]);
                }

                Console.WriteLine();
            }
        }
    }

    public class Point
    {
        public int X { get; }
        public int Y { get; }
        public int Risk { get; }

        public Point(int x,int y, int risk)
        {
            X = x;
            Y = y;
            Risk = risk;
        }

        public override string ToString()
        {
            return $"{X},{Y} - {Risk}";
        }
    };

    public class Path
    {
        public List<Point> Points { get; set; } = new List<Point>();
        public int Total => Points.Skip(1).Sum(x => x.Risk);
    }
}