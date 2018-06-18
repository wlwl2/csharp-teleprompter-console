using System;
using System.Collections.Generic;
using System.IO;

namespace TeleprompterConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = ReadFrom("sampleQuotes.txt");
            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
        }

        // The IEnumerable<T> interface is defined in the
        // System.Collections.Generic namespace.
        static IEnumerable<string> ReadFrom(string file)
        {
            string line;
            // The File class is defined in the System.IO namespace.
            using (var reader = File.OpenText(file))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}
