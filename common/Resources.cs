using System;
using System.IO;
using System.Linq;

namespace common
{
    public class Resources
    {
        public static string[] GetResourceLines(Type type, string name)
        {
            var input = new StreamReader(type.Assembly.GetManifestResourceStream(name)).ReadToEnd();
            string[] lines = input.Split(Environment.NewLine).Select(x => x.Trim()).ToArray();
            return lines;
        }

        public static int[] GetIntegerFromLines(Type owningType, string resourceName)
        {
            return Resources.GetResourceLines(owningType, resourceName).Select(x => Convert.ToInt32(x)).ToArray();
        }

        public static int[] GetSeparatedIntegers(Type owningType, string resourceName)
        {
            return Resources.GetResourceLines(owningType, resourceName).First().Split(',').Select(x => Convert.ToInt32(x)).ToArray();
        }
    }
}
