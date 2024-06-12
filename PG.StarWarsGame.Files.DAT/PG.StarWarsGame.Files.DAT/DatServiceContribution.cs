// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Attributes;
using PG.Commons.Extensibility;
using PG.StarWarsGame.Files.DAT.Binary;
using PG.StarWarsGame.Files.DAT.Services;
using PG.StarWarsGame.Files.DAT.Services.Builder.Validation;

namespace PG.StarWarsGame.Files.DAT;

/// <inheritdoc />
[Order(1100)]
public class DatServiceContribution : IServiceContribution
{
    /// <inheritdoc />
    public void ContributeServices(IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddSingleton<IDatFileService>(sp => new DatFileService(sp))
            .AddSingleton<IDatModelService>(sp => new DatModelService(sp))
            .AddTransient<IDatFileReader>(sp => new DatFileReader(sp))
            .AddTransient<IDatBinaryConverter>(sp => new DatBinaryConverter(sp))
            .AddSingleton(new EmpireAtWarKeyValidator());
    }
}