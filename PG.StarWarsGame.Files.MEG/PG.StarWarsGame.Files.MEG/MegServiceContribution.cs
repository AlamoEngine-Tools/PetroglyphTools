// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Attributes;
using PG.Commons.Extensibility;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Validation;
using PG.StarWarsGame.Files.MEG.Services;
using PG.StarWarsGame.Files.MEG.Services.Builder;
using PG.StarWarsGame.Files.MEG.Services.Builder.Normalization;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

namespace PG.StarWarsGame.Files.MEG;

/// <inheritdoc />
[Order(1000)]
public class MegServiceContribution : IServiceContribution
{
    /// <inheritdoc />
    public void ContributeServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IMegFileService>(sp => new MegFileService(sp));
        serviceCollection.AddSingleton<IMegFileExtractor>(sp => new MegFileExtractor(sp));
        serviceCollection.AddSingleton<IMegBinaryServiceFactory>(sp => new MegBinaryServiceFactory(sp));
        serviceCollection.AddSingleton<IMegVersionIdentifier>(sp => new MegVersionIdentifier(sp));
        serviceCollection.AddSingleton<IMegDataStreamFactory>(sp => new MegDataStreamFactory(sp));
        serviceCollection.AddSingleton<IVirtualMegArchiveBuilder>(_ => new VirtualMegArchiveBuilder());

        serviceCollection.AddSingleton(sp => new EmpireAtWarMegFileInformationValidator(sp));
        serviceCollection.AddSingleton(sp => new EmpireAtWarMegBuilderDataEntryValidator(sp));
        serviceCollection.AddSingleton(sp => new EmpireAtWarMegDataEntryPathNormalizer(sp));

        serviceCollection.AddSingleton(sp => new DefaultDataEntryPathNormalizer(sp));

        serviceCollection.AddSingleton<IDataEntryPathResolver>(sp => new PetroglyphRelativeDataEntryPathResolver(sp));

        serviceCollection.AddSingleton<IMegBinaryValidator>(sp => new MegBinaryValidator(sp));
        serviceCollection.AddSingleton<IFileTableValidator>(_ => new MegFileTableValidator());
        serviceCollection.AddSingleton<IMegFileSizeValidator>(_ => new MegFileSizeValidator());
    }
}