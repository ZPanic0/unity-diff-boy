using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace ConsoleApp.Queries
{
    public static class GetEditorsPath
    {
        public class Request : IRequest<string> { }

        public class Handler : IRequestHandler<Request, string>
        {
            private readonly IEnumerable<string> defaultLocations = new[] {
                @"{0}Program Files\Unity\Hub\Editor",
                @"{0}Program Files (x86)\Unity\Hub\Editor"
        };

            public async Task<string> Handle(Request request, CancellationToken cancellationToken)
            {
                Console.WriteLine("Checking for Unity Editor folder in default Unity Hub location...");
                var drive = Path.GetPathRoot(Environment.SystemDirectory);

                foreach (var defaultLocation in defaultLocations)
                {
                    var testDirectory = string.Format(defaultLocation, drive);

                    if (Directory.Exists(testDirectory))
                    {
                        Console.WriteLine($"Found at {testDirectory}");

                        return testDirectory;
                    }
                }

                Console.WriteLine("Editor folder not found.");


                var found = false;

                while (!found)
                {
                    Console.WriteLine("Please enter the Editor path: ");

                    var path = string.Empty;
                    while(!TryGetPath(ref path))
                    {
                        Console.WriteLine("Invalid path");
                        Console.WriteLine("Please enter the Editor path: ");
                    }

                    found = CheckForUnity(path);

                    if (found) return path;

                    Console.WriteLine($"Could not find Unity.exe in any folders in {path}\n");
                }

                throw new FileNotFoundException();
            }

            private bool CheckForUnity(string path)
            {
                foreach (var subDirectory in Directory.GetDirectories(path))
                {
                    Console.WriteLine($"Checking in {subDirectory}");
                    var testPath = $@"{subDirectory}\Editor\Unity.exe";

                    if (File.Exists(testPath))
                    {
                        Console.WriteLine($"Found at least one editor at {testPath}");

                        return true;
                    }
                }

                return false;
            }

            private bool TryGetPath(ref string path)
            {
                try
                {
                    path = Console.ReadLine();
                    return Directory.Exists(path);
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}
