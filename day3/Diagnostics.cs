using common;
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace day3
{
    public class Diagnostics
    {
        [TestCase("day3.sample.txt", 198, (uint)0b_1_0000)]
        [TestCase("day3.input.txt", 3633500, (uint)0b_1000_0000_0000)]
        public void Of_Power_Consumption_Is_Gamma_Times_Epsilon(string resource, int expected, uint maxBit)
        {
            var lines = GetResourceBinaries(resource);
            RunDiagnostics(lines, expected, maxBit);
        }

        [TestCase("day3.sample.txt", (uint)0b_1_0000, 230)]
        [TestCase("day3.input.txt", (uint)0b_1000_0000_0000, 4550283)]
        public void Of_Life_Support_Rating_Is_OxyGen_Times_Co2Scrub(string resource, uint maxBit, int expected)
        {
            var numbers = GetResourceBinaries(resource);

            var oxygenGenerator = 0;
            var co2Scrubber = 0;

            var oxyNumbers = numbers.ToArray();
            var co2Numbers = numbers.ToArray();
            for (uint bit = maxBit; bit >= 1; bit >>= 1)
            {
                var oxyOccurrences = CountOccurrences(oxyNumbers, bit);
                var co2Occurrences = CountOccurrences(co2Numbers, bit);

                Func<uint, bool> bitIsSet = n => (n & bit) == bit;
                Func<uint, bool> bitIsNotSet = n => (~n & bit) == bit;

                if (oxyNumbers.Length > 1)
                { 
                    oxyNumbers = oxyNumbers.Where(oxyOccurrences.mostCommon == 1 ? bitIsSet : bitIsNotSet).ToArray();
                }
                if (co2Numbers.Length > 1)
                { 
                    co2Numbers = co2Numbers.Where(co2Occurrences.mostCommon == 1 ? bitIsNotSet : bitIsSet).ToArray();
                }

                if (oxyNumbers.Length == 1 && co2Numbers.Length == 1)
                {
                    break;
                }
            }

            var actual = oxyNumbers[0] * co2Numbers[0];

            Console.WriteLine("Oxy {0} {1}", Convert.ToString(oxyNumbers[0], 2), oxyNumbers[0]);
            Console.WriteLine("Co2 {0} {1}", Convert.ToString(co2Numbers[0], 2), co2Numbers[0]);
            Console.WriteLine("Answer {0}", actual);

            Assert.AreEqual(expected, actual);
        }

        private uint[] GetResourceBinaries(string name)
        {
            return Resources.GetResourceLines(GetType(), name).Select(line => (uint)Convert.ToInt32(line, 2)).ToArray();
        }

        private static void RunDiagnostics(uint[] numbers, int expected, uint maxBit)
        {
            uint gamma = 0;
            uint epsilon = 0;
            for (uint i = maxBit; i >= 1; i >>= 1)
            {
                var (zeroes, ones, mostCommon, leastCommon) = CountOccurrences(numbers, i);

                if (mostCommon == 1)
                {
                    gamma |= i;
                }
                else
                {
                    epsilon |= i;
                }
            }

            Console.WriteLine("Gamma");
            Console.WriteLine(gamma);
            Console.WriteLine(Convert.ToString(gamma, 2));

            Console.WriteLine();
            Console.WriteLine("Epsilon");
            Console.WriteLine(epsilon);
            Console.WriteLine(Convert.ToString(epsilon, 2));

            Console.WriteLine();
            Console.WriteLine(epsilon * gamma);
            Console.WriteLine(Convert.ToString(epsilon * gamma, 2));


            Assert.AreEqual(expected, gamma * epsilon);
        }

        private static (int zeroes, int ones, uint mostCommon, uint leastCommon) CountOccurrences(uint[] numbers, uint bit)
        {
            var zeroes = 0;
            var ones = 0;
            foreach (var number in numbers)
            {
                if ((number & bit) == bit)
                {
                    ones++;
                }
                else
                {
                    zeroes++;
                }
            }

            var mostCommon = ones >= zeroes ? (uint)1 : 0;
            var leastCommon = ~mostCommon;
            return (zeroes, ones, mostCommon, leastCommon);
        }
    }
}