using System;
using System.Collections.Generic;
using System.Linq;
using common;
using NUnit.Framework;

namespace day15
{
    public class A_Star
    {
        private int[,] matrix;

        [SetUp]
        public void Setup()
        {
            var lines = Resources.GetResourceLines(typeof(A_Star), TestContext.CurrentContext.Test.Arguments[0]!.ToString());
            matrix = new int[lines.Length, lines[0].Length];
            for (var y = 0; y < lines.Length; y++)
            {
                for (var x = 0; x < lines[0].Length; x++)
                {
                    matrix[y, x] = Convert.ToInt32(lines[y][x].ToString());
                }
            }
        }

        [Test]
        [TestCase("day15.sample.txt", 60)]
        public void Finds_The_Least_Risky_Path(string resource, int maxRisk)
        {
            WriteMatrix();

            var startPath = new Path { Points = new List<Point> { new(0, 0, matrix[0, 0]) } };

            var allPaths = FindPaths(startPath, maxRisk);

            foreach (var path in allPaths)
                Console.WriteLine(String.Join(" > ", path.Points.Select(x => x.ToString())));

            Assert.Fail();
        }

        private List<(int x, int y)> neighbours = new List<(int x, int y)>
        {
            (-1, 0),
            (0, -1),
            (1, 0),
            (0, 1)
        };

        private List<Path> FindPaths(Path path, int maxRisk)
        {
            var pt = path.Last();
            var newPaths = new List<Path>();
            var maxY = matrix.GetUpperBound(0);
            var maxX = matrix.GetUpperBound(1);
            var theseNeighbours = neighbours.Select(rel =>
                {
                    var x = pt.X + rel.x;
                    var y = pt.Y + rel.y;

                    if (y >= 0 && y <= maxY && x >= 0 && x <= maxX)
                    {
                        return new Point(x, y, matrix[y, x]);
                    }

                    return new Point(-1, -1, -1);
                })
                .Where(p => p.X > -1 && !path.Contains(p))
                .OrderBy(p => p.Risk)
                .ToList();

            for (var i = 0; i < theseNeighbours.Count; i++)
            {
                Point candidate = theseNeighbours[i];
                if (!path.Contains(candidate))
                {
                    var pathHere = path.Concat(candidate);
                    if (pathHere.Total < maxRisk && !(candidate.X == maxX && candidate.Y == maxY))
                    {
                        newPaths.AddRange(FindPaths(pathHere, maxRisk));
                    }
                    else
                    {
                        newPaths.Add(pathHere);
                    }
                }

            }

            return newPaths;
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

    public record Point(int X, int Y, int Risk)
    {
        public override string ToString()
        {
            return $"{X},{Y} - {Risk}";
        }
    };

    public class Path
    {
        public List<Point> Points { get; set; }
        public int Total => Points.Skip(1).Sum(x => x.Risk);

        public Point Last() => Points.Last();
        public bool Contains(Point point) => Points.Contains(point);

        public Path Concat(Point point) => Concat(new[] { point });

        private Path Concat(Point[] points) =>
            new Path
            {
                Points = Points.Concat(points).ToList()
            };
    }
}