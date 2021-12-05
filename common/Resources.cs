using System;
using System.IO;

namespace common
{
    public class Resources
    {
        public static string[] GetResourceLines(Type type, string name)
        {
            var input = new StreamReader(type.Assembly.GetManifestResourceStream(name)).ReadToEnd();
            string[] lines = input.Split(Environment.NewLine);
            return lines;
        }
    }
}
