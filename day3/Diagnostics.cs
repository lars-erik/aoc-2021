using common;
using System;
using NUnit.Framework;

namespace day3
{
    public class Diagnostics
    {
        [Test]
        public void For_Sample_Yields_Gamma_Times_Epsilon()
        {
            var lines = Resources.GetResourceLines(GetType(), "day3.sample.txt");
            RunDiagnostics(lines, 198, 0b_1_0000);
        }

        [Test]
        public void For_Input_Yields_Gamma_Times_Epsilon()
        {
            var lines = Resources.GetResourceLines(GetType(), "day3.input.txt");
            RunDiagnostics(lines, 3633500, 0b_1000_0000_0000);
        }

        private static void RunDiagnostics(string[] lines, int expected, int maxBit)
        {
            uint gamma = 0;
            uint epsilon = 0;
            for (uint i = 1; i <= maxBit; i <<= 1)
            {
                var zeroes = 0;
                var ones = 0;
                foreach (var line in lines)
                {
                    var lineNum = (uint)Convert.ToInt32(line, 2);
                    if ((lineNum & i) == i)
                    {
                        ones++;
                    }
                    else
                    {
                        zeroes++;
                    }
                }

                if (ones > zeroes)
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
    }
}