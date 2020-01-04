using ConsoleApp.Commands;
using ConsoleApp.Queries;
using MediatR;
using System.IO;
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

        public async Task Execute()
        {
            var sourcePath = @"C:\Program Files\Unity\Hub\Editor\2019.2.16f1";
            var targetPath = @"C:\Program Files\Unity\Hub\Editor\2019.2.17f1";

            var result = await mediator.Send(new GetVersionComparison.Request(sourcePath, targetPath));

            await mediator.Send(new GenerateDiff.Request(
                sourcePath,
                targetPath,
                result.AddedFolders,
                result.AddedFiles,
                result.ModifiedFiles,
                result.RemovedFolders,
                result.RemovedFiles));

            var workingPath = @"C:\Program Files\Unity\Hub\Editor\2019.2.16f1 in progress";
            Directory.CreateDirectory(workingPath);

            await mediator.Send(new CopyFolder.Request(sourcePath, workingPath));

            await mediator.Send(new ApplyDiff.Request(workingPath, $@"{Directory.GetCurrentDirectory()}\2019.2.16f1 to 2019.2.17f1.zip"));
        }
    }
}
