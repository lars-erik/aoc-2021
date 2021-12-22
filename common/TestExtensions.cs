using System;
using Newtonsoft.Json;

namespace common
{
    public static class TestExtensions
    {
        public static void Dump(this object obj)
        {
            Console.WriteLine(JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
        }
    }
}
