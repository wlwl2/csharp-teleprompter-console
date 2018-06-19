using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TeleprompterConsole
{
    class Program
    {
        // In a console application’s Main method, you cannot use the await
        // operator.
        // If you use C# 7.1 or later, you can create console applications
        // with async Main method.
        static void Main(string[] args)
        {
            var lines = ReadFrom("sampleQuotes.txt");
            foreach (var line in lines)
            {
                Console.Write(line);
                if (!string.IsNullOrWhiteSpace(line))
                {
                    var pause = Task.Delay(200);
                    // Synchronously waiting on a task is an
                    // anti-pattern. This will get fixed in later
                    // steps.
                    pause.Wait();
                }
            }
            ShowTeleprompter().Wait();

        }

        private static async Task ShowTeleprompter()
        {
            var words = ReadFrom("sampleQuotes.txt");
            foreach (var word in words)
            {
                Console.Write(word);
                if (!string.IsNullOrWhiteSpace(word))
                {
                    // You can imagine that this method returns when it
                    // reaches an await.
                    // The returned Task indicates that the work has not
                    // completed.
                    await Task.Delay(200);
                }
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
                    var words = line.Split(' ');

                    var lineLength = 0;
                    // return single words instead of entire lines
                    foreach (var word in words)
                    {
                        yield return word + " ";
                        lineLength += word.Length + 1;
                        if (lineLength > 70)
                        {
                            // Environment.NewLine returns a string containing
                            // "\r\n" for non-Unix platforms, or a string
                            // containing "\n" for Unix platforms.
                            yield return Environment.NewLine;
                            lineLength = 0;
                        }
                    }
                    yield return Environment.NewLine;
                }
            }
        }

        private static async Task GetInput()
        {
            var delay = 200;

            // lambda expression to represent an Action delegate that reads a
            // key from the Console and modifies a local variable representing
            // the delay when the user presses the ‘<’ or ‘>’ keys.
            Action work = () =>
            {
                do {
                    var key = Console.ReadKey(true);
                    if (key.KeyChar == '>')
                    {
                        delay -= 10;
                    }
                    else if (key.KeyChar == '<')
                    {
                        delay += 10;
                    }
                } while (true);
            };
            await Task.Run(work);
        }
    }
}
