using ConsoleApp.Commands;
using ConsoleApp.Queries;
using MediatR;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class Application
    {
        private readonly IMediator mediator;

        public Application(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task Execute(string[] args)
        {
            if (!args.Any())
            {
                PrintDocumentation();
                return;
            }

            switch (args.First().ToLower())
            {
                case "/c":
                case "/create":
                    await CreateDelta(args);
                    break;
                case "/a":
                case "/apply":
                    await ApplyDelta(args);
                    break;
                case "/h":
                default:
                    PrintDocumentation();
                    break;
            }
        }
        private void PrintDocumentation()
        {
            var builder = new StringBuilder();

            builder.AppendLine("To create a delta:");
            builder.AppendLine($"{AppDomain.CurrentDomain.FriendlyName} [/c | /create] dirname1 dirname2");
            builder.AppendLine();
            builder.AppendLine("dirname1 \t The full path of the base installation to generate the delta patch from.");
            builder.AppendLine("dirname2 \t The full path of the target installation the delta patch should upgrade to.");
            builder.AppendLine();

            builder.AppendLine("To apply a delta:");
            builder.AppendLine($"{AppDomain.CurrentDomain.FriendlyName} [/a | /apply] dirname1 filename1");
            builder.AppendLine();
            builder.AppendLine("dirname1 \t The full path of the target installation to upgrade.");
            builder.AppendLine("filename1 \t The full path of the delta patch to apply.");
            builder.AppendLine();

            Console.Write(builder.ToString());
        }

        private async Task CreateDelta(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Missing arguments.\n\n");
                PrintDocumentation();
                return;
            }

            if (!Directory.Exists(args[1]))
            {
                Console.WriteLine($"Path '{args[1]}' does not exist.\n\n");
                PrintDocumentation();
                return;
            }

            if (!Directory.Exists(args[2]))
            {
                Console.WriteLine($"Path '{args[2]}' does not exist.\n\n");
                PrintDocumentation();
                return;
            }

            var result = await mediator.Send(new GetVersionComparison.Request(args[1], args[2]));

            await mediator.Send(new GenerateDiff.Request(
                args[1],
                args[2],
                result.AddedFolders,
                result.AddedFiles,
                result.ModifiedFiles,
                result.RemovedFolders,
                result.RemovedFiles));
        }

        private async Task ApplyDelta(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Missing arguments.\n\n");
                PrintDocumentation();
                return;
            }

            if (!Directory.Exists(args[1]))
            {
                Console.WriteLine($"Path '{args[1]}' does not exist.\n\n");
                PrintDocumentation();
                return;
            }

            if (!File.Exists(args[2]))
            {
                Console.WriteLine($"File '{args[2]}' does not exist.\n\n");
                PrintDocumentation();
                return;
            }

            var targetPath = args[1] + " WIP";

            await mediator.Send(new CopyFolder.Request(args[1], targetPath));

            await mediator.Send(new ApplyDiff.Request(targetPath, args[2]));
        }
    }
}
