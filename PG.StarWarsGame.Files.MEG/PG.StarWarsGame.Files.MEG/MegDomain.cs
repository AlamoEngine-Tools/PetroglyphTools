// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Validation;
using PG.StarWarsGame.Files.MEG.Services;
using PG.StarWarsGame.Files.MEG.Services.Builder;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

namespace PG.StarWarsGame.Files.MEG;

/// <summary>
/// Provides methods to initialize this library.
/// </summary>
public static class MegDomain
{
    /// <summary>
    /// Adds the required services for this library to the service collection.
    /// </summary>
    /// <param name="serviceCollection">The service collection to populate.</param>
    public static void RegisterServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IMegFileService>(sp => new MegFileService(sp));
        serviceCollection.AddSingleton<IMegFileExtractor>(sp => new MegFileExtractor(sp));
        serviceCollection.AddSingleton<IMegBinaryServiceFactory>(sp => new MegBinaryServiceFactory(sp));
        serviceCollection.AddSingleton<IMegVersionIdentifier>(sp => new MegVersionIdentifier(sp));
        serviceCollection.AddSingleton<IMegDataStreamFactory>(sp => new MegDataStreamFactory(sp));
        serviceCollection.AddSingleton<IVirtualMegArchiveBuilder>(sp => new VirtualMegArchiveBuilder());

        serviceCollection.AddSingleton(sp => new EmpireAtWarMegFileInformationValidator(sp));
        serviceCollection.AddSingleton(sp => new EmpireAtWarMegDataEntryValidator(sp));

        serviceCollection.AddSingleton(sp => new PetroglyphDataEntryPathNormalizer(sp));
        serviceCollection.AddSingleton(sp => new DefaultDataEntryPathNormalizer(sp));

        serviceCollection.AddSingleton<IDataEntryPathResolver>(sp => new PetroglyphRelativeDataEntryPathResolver(sp));
        
        serviceCollection.AddTransient<IMegBinaryValidator>(sp => new MegBinaryValidator(sp));
        serviceCollection.AddTransient<IFileTableValidator>(_ => new MegFileTableValidator());
        serviceCollection.AddTransient<IMegFileSizeValidator>(_ => new MegFileSizeValidator());
    }
}