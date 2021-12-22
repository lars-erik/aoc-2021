using System;
using System.Collections.Generic;
using System.Linq;
using common;
using Csg;
using NUnit.Framework;
using Resources = common.Resources;
using Vector3 = System.Numerics.Vector3;

namespace day22
{
    [TestFixture]
    public class Reactor_Cubes
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase("SmallSample", 39)]
        [TestCase("LargerSample", 590784)]
        public void Are_Toggled(string resource, long expectedCubes)
        {
            var lines = Resources.GetResourceLines(typeof(Reactor_Cubes), $"day22.{resource}.txt");
            var sets = new List<(string state, Dictionary<string, (int min, int max)> axes)>();
            foreach (var line in lines)
            {
                var mainParts = line.Split(" ");
                var state = mainParts[0];
                var lineRanges = mainParts[1].Split(",").Select(r =>
                {
                    var axisParts = r.Split("=");
                    var axis = axisParts[0];
                    var range = axisParts[1].Split("..").Select(int.Parse).OrderBy(x => x).ToArray();
                    return (axis, range:(min:range[0], max:range[1]));
                })
                    .ToDictionary(x => x.axis, x => x.range);
                var set = (state, axes: lineRanges);
                sets.Add(set);
            }

            var cubes = new List<Vector3>();
            foreach (var set in sets)
            {


                for (var x = set.axes["x"].min; x <= set.axes["x"].max; x++)
                {
                    for (var y = set.axes["y"].min; y <= set.axes["y"].max; y++)
                    {
                        for (var z = set.axes["z"].min; z <= set.axes["z"].max; z++)
                        {
                            if (x < -50 || x > 50 || y < -50 || y > 50 || z < -50 || z > 50)
                            {
                                continue;
                            }

                            var vec = new Vector3(x, y, z);
                            if (set.state == "on" && !cubes.Contains(vec))
                            {
                                cubes.Add(vec);
                            }

                            if (set.state == "off" && cubes.Contains(vec))
                            {
                                cubes.Remove(vec);
                            }
                        }
                    }

                }
            }

            cubes.Take(100).Dump();

            Console.WriteLine(cubes.Count);

            Assert.AreEqual(expectedCubes, cubes.Count);
        }

        [Test]
        public void This_Csg_Lib_Works()
        {
            var cubeA = Solids.Cube(size:new Vector3D(2, 1, 1), center: new Vector3D(1, .5, .5));

            Console.WriteLine("A");
            foreach (var polygon in cubeA.Polygons)
            {
                Console.WriteLine(polygon.BoundingBox.Min + ", " + polygon.BoundingBox.Max);
            }
            Console.WriteLine();
            
            var cubeB = Solids.Cube(size:new Vector3D(1, 1, 1), center:new Vector3D(1.5, 0.5, 0.5));

            Console.WriteLine("B");
            foreach (var polygon in cubeB.Polygons)
            {
                Console.WriteLine(polygon.BoundingBox.Min + ", " + polygon.BoundingBox.Max);
            }
            Console.WriteLine();

            var cubeC = cubeA.Intersect(cubeB);
            Console.WriteLine("C");
            var vertices = cubeC.Polygons.SelectMany(p => p.Vertices.Select(v => v.Pos)).Distinct();
            foreach (var vert in vertices)
            {
                Console.WriteLine(vert);
            }
            Console.WriteLine();



        }
    }
}