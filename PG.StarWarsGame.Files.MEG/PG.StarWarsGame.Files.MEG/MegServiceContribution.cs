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
internal sealed class MegServiceContribution : IServiceContribution
{
    /// <inheritdoc />
    public void ContributeServices(IServiceCollection serviceCollection)
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