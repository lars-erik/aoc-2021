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
                    if (e.ContainsKey(x[0]))
                        e[x[0]].Add(x[1]);
                    else
                        e.Add(x[0], new List<string>(new[] { x[1] }));
                    if (e.ContainsKey(x[1]))
                        e[x[1]].Add(x[0]);
                    else
                        e.Add(x[1], new List<string>(new[] { x[0] }));
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

            var paths = FindPaths(new[] { "start" })
                .Where(x => x.Last() == "end");

            foreach (var path in paths)
            {
                Console.WriteLine(String.Join(" -> ", path));
            }

            Assert.AreEqual(expectedPaths, paths.Count());
        }

        private IEnumerable<IEnumerable<string>> FindPaths(IEnumerable<string> path, int maxVisits = 1)
        {
            var nextEdges = edges[path.Last()];
            var newPaths = new List<IEnumerable<string>>();
            foreach (var edge in nextEdges)
            {
                var isLarge = edge[0] < 'a';

                if (isLarge || (path.Count(x => x == edge) < maxVisits))
                {
                    var newPath = path.Concat(new[] { edge });
                    if (edge == "end")
                    {
                        newPaths.Add(newPath);
                    }
                    else
                    {
                        var subPaths = FindPaths(newPath);
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
}