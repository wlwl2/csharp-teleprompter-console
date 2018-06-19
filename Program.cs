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
            // var lines = ReadFrom("sampleQuotes.txt");
            // foreach (var line in lines)
            // {
            //     Console.Write(line);
            //     if (!string.IsNullOrWhiteSpace(line))
            //     {
            //         var pause = Task.Delay(200);
            //         // Synchronously waiting on a task is an
            //         // anti-pattern. This will get fixed in later
            //         // steps.
            //         pause.Wait();
            //     }
            // }
            RunTeleprompter().Wait();

        }

        private static async Task ShowTeleprompter(TelePrompterConfig config)
        {
            var words = ReadFrom("sampleQuotes.txt");
            foreach (var line in words)
            {
                // Console.Write(config.DelayInMilliseconds);
                Console.Write(line);
                if (!string.IsNullOrWhiteSpace(line))
                {
                    await Task.Delay(config.DelayInMilliseconds);
                }
            }
            config.SetDone();
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

        private static async Task GetInput(TelePrompterConfig config)
        {
            Action work = () =>
            {
                do {
                    var key = Console.ReadKey(true);
                    if (key.KeyChar == '>')
                        config.UpdateDelay(-10);
                    else if (key.KeyChar == '<')
                        config.UpdateDelay(10);
                } while (!config.Done);
            };
            await Task.Run(work);
        }

        private static async Task RunTeleprompter()
        {
            var config = new TelePrompterConfig();
            var displayTask = ShowTeleprompter(config);

            var speedTask = GetInput(config);
            await Task.WhenAny(displayTask, speedTask);
        }
    }
}
