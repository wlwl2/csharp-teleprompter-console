# TeleprompterConsole (.NET Core 5/VSCode)

`dotnet --version` should be version 5 (e.g. 5.0.401).

Type in `dotnet run` in the VSCode terminal in the 
root/main folder to run the application.

Use `Ctrl-C` to stop the teleprompter.

This project shows:

- The basics of the .NET Core CLI
- The structure of a C# Console Application
- Console I/O
- The basics of File I/O APIs in .NET
- The basics of the Task-based Asynchronous Programming in .NET

Use VSCode.

https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/console-teleprompter

## Creating a new console app

1. `dotnet new console`
2. `dotnet restore`

## Program.cs

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TeleprompterConsole {
    class Program {
        static void Main (string[] args) {
            RunTeleprompter().Wait();
        }

        private static async Task ShowTeleprompter (TelePrompterConfig config) {
            var words = ReadFrom("sampleQuotes.txt");
            foreach (var line in words) {
                Console.Write(line);
                if (!string.IsNullOrWhiteSpace(line)) {
                    await Task.Delay(config.DelayInMilliseconds);
                }
            }
            config.SetDone();
        }

        static IEnumerable<string> ReadFrom (string file) {
            string line;
            using (var reader = File.OpenText(file)) {
                while ((line = reader.ReadLine()) != null) {
                    var words = line.Split(' ');
                    var lineLength = 0;
                    foreach (var word in words) {
                        yield return word + " ";
                        lineLength += word.Length + 1;
                        if (lineLength > 70) {
                            yield return Environment.NewLine;
                            lineLength = 0;
                        }
                    }
                    yield return Environment.NewLine;
                }
            }
        }

        private static async Task GetInput (TelePrompterConfig config) {
            Action work = () => {
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

        private static async Task RunTeleprompter () {
            var config = new TelePrompterConfig();
            var displayTask = ShowTeleprompter(config);

            var speedTask = GetInput(config);
            await Task.WhenAny(displayTask, speedTask);
        }
    }
}
```