using common;
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace day1
{
    public class Measuring_Depth
    {
        [Test]
        public void Counts_Increases()
        {
            var measures = Resources.GetIntegerFromLines(GetType(), "day1.p1.txt");
            int increases = CountIncreases(measures);

            Assert.AreEqual(1477, increases);
        }

        [Test]
        public void Counts_Group_Increases()
        {
            var measures = Resources.GetIntegerFromLines(GetType(), "day1.p1.txt");
            var sums = new List<int>();
            for (var i = 0; i<measures.Length-2; i++) {
                sums.Add(measures[i] + measures[i+1] + measures[i+2]);
            }
            
            var increases = CountIncreases(sums.ToArray());
                 
            Assert.AreEqual(1523, increases);
        }

        public static int CountIncreases(int[] measures)
        {
            var increases = 0;
            for (var i = 1; i < measures.Length; i++)
            {
                if (measures[i] > measures[i - 1])
                {
                    increases++;
                }
            }

            return increases;
        }
    }
}