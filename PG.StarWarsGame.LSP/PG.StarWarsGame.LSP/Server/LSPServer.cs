// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server;
using PG.StarWarsGame.LSP.Server.Buffering;

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
            );
            await server.WaitForExit;
        }

        static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IBufferManager, SimpleBufferManager>();
        }
    }
}
