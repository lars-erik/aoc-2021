using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using common;
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
            var regions = CreateRegions(resource);

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

                    var result = intersecting.Intersect(region);
                    newRegions.Remove(intersecting);
                    newRegions.AddRange(result);
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
        public void Creates_Paths_Along_X()
        {
            var a = new Region(0, 0, 0, 1, 0, 0);
            var b = new Region(1, 0, 0, 1, 0, 0);
            var c = new Region(1, 0, 0, 2, 0, 0);
            var d = new Region(2, 0, 0, 3, 0, 0);

            Console.WriteLine("A");
            a.XPaths(a).Select(x => x.ToString()).Dump();
            Console.WriteLine("AB");
            a.XPaths(b).Select(x => x.ToString()).Dump();
            Console.WriteLine("AC");
            a.XPaths(c).Select(x => x.ToString()).Dump();
            Console.WriteLine("AD");
            a.XPaths(d).Select(x => x.ToString()).Dump();
            Console.WriteLine("BC");
            b.XPaths(c).Select(x => x.ToString()).Dump();
        }

        [Test]
        public void Creates_New_Boxes_When_Intersecting_Inside()
        {
            var a = new Region(0, 0, 0, 1, 0, 0);
            var b = new Region(1, 0, 0, 1, 0, 0);
            var newBoxes = a.Intersect(b);
            newBoxes.Select(x => x.ToString()).Dump();
            Assert.Fail();
        }

        [Test]
        public void Creates_New_Boxes_When_Overlapping()
        {
            var a = new Region(0, 0, 0, 1, 0, 0);
            var b = new Region(1, 0, 0, 2, 0, 0);
            var newBoxes = a.Intersect(b);
            newBoxes.Select(x => x.ToString()).Dump();
            Assert.Fail();
        }

        [Test]
        public void Creates_New_Boxes_When_Overlapping_2D()
        {
            var a = new Region(0, 0, 0, 1, 1, 0);
            var b = new Region(1, 2, 0, 2, 2, 0);
            var newBoxes = a.Intersect(b);
            newBoxes.Select(x => x.ToString()).Dump();
            Assert.AreEqual(7, newBoxes.Length);
        }

        [Test]
        public void Creates_Vertices_For_Sample_Step_1()
        {
            var a = new Region(10, 10, 10, 12, 12, 12);
            var b = new Region(11, 11, 11, 13, 13, 13);

            a.XPaths(b).Select(v => v.ToString()).Dump();
            a.YPaths(b).Select(v => v.ToString()).Dump();
            a.ZPaths(b).Select(v => v.ToString()).Dump();
        }

        [Test]
        public void Creates_New_Boxes_For_Sample_Step_1()
        {
            var a = new Region(10, 10, 10, 12, 12, 12);
            var b = new Region(11, 11, 11, 13, 13, 13);

            var newBoxes = a.Intersect(b);

            newBoxes.Select(x => x.ToString()).Dump();
            Console.WriteLine(newBoxes.Length);
            Assert.Fail();
        }

        [Test]
        public void Doesnt_Intersect()
        {
            var a = new Region(0, 0, 0, 3, 2, 2);
            var b = new Region(3, 1, 1, 4, 3, 3);
            var intersects = a.Intersects(b) && b.Intersects(a);
            Assert.AreEqual(false, intersects);
        }

        [Test]
        public void Instersected_Regions_Create_New_Split_Regions()
        {
            var a = new Region(0, 0, 0, 2, 1, 1);
            var b = new Region(1, 0, 0, 3, 1, 1);
            var intersect = a.Intersect(b);
            Assert.AreEqual(3, intersect.Count());
        }

        private static List<Region> CreateRegions(string resource)
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
                        return (axis, range: (min: range[0], max: range[1]));
                    })
                    .ToDictionary(x => x.axis, x => x.range);
                regions.Add(new Region(lineRanges){State = state});
            }

            return regions;
        }
    }

    public class Region
    {
        private float minX;
        private float maxX;
        private float minY;
        private float maxY;
        private float minZ;
        private float maxZ;
        private Vector3 size;
        private Vector3 center;
        public string State { get; set; }

        public Region(float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
        {
            this.minX = minX - .5f;
            this.minY = minY - .5f;
            this.minZ = minZ - .5f;
            this.maxX = maxX + .5f;
            this.maxY = maxY + .5f;
            this.maxZ = maxZ + .5f;
            size = new Vector3(MathF.Abs(maxX - minX) + 1, MathF.Abs(maxY - minY) + 1, MathF.Abs(maxZ - minZ) + 1);
            center = new Vector3(minX + size.X / 2, minY + size.Y / 2, minZ + size.Z / 2);
        }

        public Region(Dictionary<string, (int min, int max)> axis)
            : this(axis["x"].min, axis["y"].min, axis["z"].min, axis["x"].max, axis["y"].max, axis["z"].max)
        {
        }

        public bool Intersects(Region other)
        {
            return minX > other.minX && minX < other.maxX
                && minY > other.minY && minY < other.maxY
                && minZ > other.minZ && minZ < other.maxZ
                || maxX > other.minX && maxX < other.maxX
                && maxY > other.minY && maxY < other.maxY
                && maxZ > other.minZ && maxZ < other.maxZ;
        }

        public Region[] Intersect(Region other)
        {
            var yPaths = YPaths(other);
            var zPaths = ZPaths(other);
            var xPaths = XPaths(other);
            List<Region> newRegions = new List<Region>();
            foreach(var xPath in xPaths)
            {
                SplitX(newRegions, xPath);
                other.SplitX(newRegions, xPath);
            }

            foreach (var yPath in yPaths)
            {
                
            }
            //foreach (var yPath in yPaths)
            //{
            //    foreach (var zPath in zPaths)
            //    {
            //        var newRegion = new Region(xPath.X + .5f, yPath.X + .5f, zPath.X + .5f, xPath.Y - .5f, yPath.Y - .5f, zPath.Y - .5f);
            //        newRegions.Add(newRegion);
            //    }
            //}

            return newRegions.Distinct().ToArray();
        }

        private void SplitX(List<Region> newRegions, Vector2 xPath)
        {
            if (xPath.X >= minX && xPath.Y <= maxX)
            {
                newRegions.Add(new Region(xPath.X, minY, minZ, xPath.Y, maxY, maxZ));
            }
        }

        public Vector2[] XPaths(Region other)
        {
            return Paths(minX, maxX, other.minX, other.maxX);
        }

        public Vector2[] YPaths(Region other)
        {
            return Paths(minY, maxY, other.minY, other.maxY);
        }

        public Vector2[] ZPaths(Region other)
        {
            return Paths(minZ, maxZ, other.minZ, other.maxZ);
        }

        private static Vector2[] Paths(float min, float max, float otherMin, float otherMax)
        {
            if (otherMin >= max || min >= otherMax)
            {
                return new[]
                {
                    new Vector2(min, max),
                    new Vector2(otherMin, otherMax)
                };
            }

            var points = new[] {min, max, otherMin, otherMax}.OrderBy(x => x).ToArray();
            return new Vector2[]
                {
                    new(points[0], points[1]),
                    new(points[1], points[2]),
                    new(points[2], points[3])
                }
                .Where(v => Math.Abs(v.X - v.Y) > 0)
                .Distinct()
                .ToArray();
        }

        public override string ToString()
        {
            return $"<<{minX}, {minY}, {minZ}>, <{maxX}, {maxY}, {maxZ}>>";
        }

        protected bool Equals(Region other)
        {
            return minX.Equals(other.minX) && maxX.Equals(other.maxX) && minY.Equals(other.minY) && maxY.Equals(other.maxY) && minZ.Equals(other.minZ) && maxZ.Equals(other.maxZ);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Region) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(minX, maxX, minY, maxY, minZ, maxZ);
        }
    }
}