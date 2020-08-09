using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server;
using PG.StarWarsGame.LSP.Server.Buffering;
using PG.StarWarsGame.LSP.Server.Handlers.Engine;

namespace PG.StarWarsGame.LSP.Server
{
    internal static class LSPServer
    {
        internal static async Task Main(string[] args)
        {
            ILanguageServer server = await LanguageServer.From(options =>
                options
                    .WithInput(Console.OpenStandardInput())
                    .WithOutput(Console.OpenStandardOutput())
                    .WithServices(ConfigureServices)
                    .WithHandler<GameConstantsFileDocumentSyncHandler>()
            );
            await server.WaitForExit;
        }

        static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IBufferManager, SimpleBufferManager>();
        }
    }
}
