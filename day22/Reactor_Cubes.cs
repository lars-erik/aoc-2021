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
        //[TestCase("LargerSample", 590784)]
        public void Are_Toggled(string resource, long expectedCubes)
        {
            var lines = Resources.GetResourceLines(typeof(Reactor_Cubes), $"day22.{resource}.txt");
            var regions = new List<Region>();
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
                regions.Add(new Region(state, lineRanges));
            }

            var newRegions = new List<Region>();
            for (var i = 0; i < regions.Count; i++)
            {
                var region = regions[i];

                var intersections = newRegions.Where(x => x.Intersects(region));

                if (region.State == "off")
                {
                    foreach (var toTurnOff in intersections)
                    {

                    }
                }
                else
                {
                    
                }

                if (newRegions.Any())
                {
                    var intersecting = newRegions.First();

                    var newRegion = intersecting.Add(region);
                    newRegions.Remove(intersecting);
                    newRegions.Add(newRegion);
                }
                else if (region.State == "on")
                {
                    newRegions.Add(region);
                }



                //var intersecting = newRegions.Where(x => x.Intersects(region));
                //if (!intersecting.Any())
                //{
                //    if (region.State == "on")
                //    {
                //        newRegions.Add(region);
                //    }
                //}
            }

            var cubeCount = 0;

            Assert.AreEqual(expectedCubes, cubeCount);
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

        [Test]
        public void Size_Of_Intersection_Of_Separate_Is_Zero()
        {
            var cubeA = Solids.Cube(new Vector3D(1, 1, 1), new Vector3D(0, 0, 0));
            var cubeB = Solids.Cube(new Vector3D(1, 1, 1), new Vector3D(10, 0, 0));
            var intersect = cubeA.Intersect(cubeB);
            Assert.AreEqual(0, intersect.Polygons.Count);
        }
    }

    public class Region
    {
        private Solid solid;
        public string State { get; }

        public Region(string state, Dictionary<string, (int min, int max)> axis)
        {
            State = state;

            double minX = axis["x"].min - .5;
            double maxX = axis["x"].max + .5;
            double minY = axis["y"].min - .5;
            double maxY = axis["y"].max + .5;
            double minZ = axis["z"].min - .5;
            double maxZ = axis["z"].max + .5;
            var size = new Vector3D(Math.Abs(maxX - minX), Math.Abs(maxY - minY), Math.Abs(maxZ - minZ));
            var center = new Vector3D(minX + (maxX - minX) / 2, minY + (maxY - minY) / 2, minZ + (maxZ - minZ) / 2);
            solid = Solids.Cube(size, center);
        }

        public Region(string state, Solid solid)
        {
            State = state;
            this.solid = solid;
        }

        public bool Intersects(Region other)
        {
            return solid.Intersect(other.solid).Polygons.Count > 0;
        }

        public Region Add(Region other)
        {
            return new Region(State, solid.Union(other.solid));
        }
    }
}