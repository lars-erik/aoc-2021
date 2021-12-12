using System;
using System.Collections.Generic;
using System.Linq;
using common;
using Newtonsoft.Json;
using NUnit.Framework;

namespace day12
{
    [TestFixture]
    public class Mapping_Caves
    {
        private Dictionary<string, List<string>> edges;

        [SetUp]
        public void Setup()
        {
            edges = Resources.GetResourceLines(typeof(Mapping_Caves), TestContext.CurrentContext.Test.Arguments[0]!.ToString())
                .Select(x => x.Split('-'))
                .Aggregate(new Dictionary<string, List<string>>(), (e, x) =>
                {
                    if (x[0] != "end" && x[1] != "start")
                    {
                        if (e.ContainsKey(x[0]))
                            e[x[0]].Add(x[1]);
                        else
                            e.Add(x[0], new List<string>(new[] { x[1] }));
                    }

                    if (x[1] != "end" && x[0] != "start")
                    {
                        if (e.ContainsKey(x[1]))
                            e[x[1]].Add(x[0]);
                        else
                            e.Add(x[1], new List<string>(new[] {x[0]}));
                    }

                    return e;
                });
        }

        [Test]
        [TestCase("day12.smallsample.txt", 10)]
        [TestCase("day12.slightlylargersample.txt", 19)]
        [TestCase("day12.evenlargersample.txt", 226)]
        [TestCase("day12.input.txt", 3450)]
        public void Shows_A_Bunch_Of_Paths(string resource, int expectedPaths)
        {
            Console.WriteLine(JsonConvert.SerializeObject(edges, Formatting.Indented));

            var paths = FindPaths(new CavePath("start"))
                .Where(x => x.Last() == "end");

            foreach (var path in paths)
            {
                Console.WriteLine(path);
            }

            Assert.AreEqual(expectedPaths, paths.Count());
        }

        [Test]
        [TestCase("day12.smallsample.txt", 36)]
        [TestCase("day12.slightlylargersample.txt", 103)]
        [TestCase("day12.evenlargersample.txt", 3509)]
        [TestCase("day12.input.txt", 96528, Category = "Slow")]
        public void Allows_For_Longer_Trips_Visiting_Caves_Twice(string resource, int expectedPaths)
        {
            Console.WriteLine(JsonConvert.SerializeObject(edges, Formatting.Indented));

            var paths = FindPaths(new CavePath("start"), 2)
                .Where(x => x.Last() == "end");

            foreach (var path in paths)
            {
                Console.WriteLine(path);
            }

            Assert.AreEqual(expectedPaths, paths.Count());
        }

        private IEnumerable<CavePath> FindPaths(CavePath path, int maxVisits = 1)
        {
            var nextEdges = edges[path.Last()];
            var newPaths = new List<CavePath>();
            foreach (var edge in nextEdges)
            {
                var isLarge = edge[0] < 'a';

                var existingCount = path.Count(edge);
                bool allowAdd;
                if (path.MaxVisitIsSpent)
                {
                    allowAdd = existingCount == 0;
                }
                else
                {
                    allowAdd = existingCount < maxVisits;
                }
                if (isLarge || allowAdd)
                {
                    var newPath = path.Concat(edge);
                    if (edge == "end")
                    {
                        newPaths.Add(newPath);
                    }
                    else
                    {
                        if (!isLarge && existingCount == maxVisits - 1)
                        {
                            newPath.SetSpent();
                        }

                        var subPaths = FindPaths(newPath, maxVisits);
                        newPaths.AddRange(subPaths);
                    }
                }
            }

            if (newPaths.Count == 0)
            {
                return new[] { path };
            }
            return newPaths;
        }
    }

    public class CavePath
    {
        private IEnumerable<string> path;
        private bool spent;

        public CavePath(string start) : this(new[] { start })
        {
        }

        public CavePath(IEnumerable<string> path)
        {
            this.path = path;
        }

        public bool MaxVisitIsSpent => spent;
        public string Last() => path.Last();
        public int Count(string cave) => path.Count(x => x == cave);

        public CavePath Concat(string edge)
        {
            return new CavePath(path.Concat(new[] { edge }))
            {
                spent = this.spent
            };
        }

        public override string ToString()
        {
            return String.Join(" -> ", path);
        }

        public void SetSpent()
        {
            spent = true;
        }
    }
}