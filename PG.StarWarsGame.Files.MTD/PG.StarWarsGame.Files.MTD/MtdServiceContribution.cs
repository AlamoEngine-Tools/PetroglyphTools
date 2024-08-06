// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.MTD.Binary;
using PG.StarWarsGame.Files.MTD.Services;

namespace PG.StarWarsGame.Files.MTD;

public static class MtdServiceContribution 
{
    public static void AddMtdServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IMtdFileService>(sp => new MtdFileService(sp));
        serviceCollection.AddSingleton<IMtdBinaryConverter>(sp => new MtdBinaryConverter(sp));
    }
}