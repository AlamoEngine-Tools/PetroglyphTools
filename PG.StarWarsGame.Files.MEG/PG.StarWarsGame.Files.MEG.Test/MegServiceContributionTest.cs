using AnakinRaW.CommonUtilities.Testing.Extensions;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.MEG.Binary;
using PG.StarWarsGame.Files.MEG.Services;
using PG.StarWarsGame.Files.MEG.Services.Builder;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test;

public class MegServiceContributionTest : CommonMegTestBase
{
    [Fact]
    public void SupportMEG_Registers()
    {
        Assert.DoesNotThrow(() => ServiceProvider.GetRequiredService<IMegFileService>());
        Assert.DoesNotThrow(() => ServiceProvider.GetRequiredService<IMegFileExtractor>());
        Assert.DoesNotThrow(() => ServiceProvider.GetRequiredService<IMegBinaryServiceFactory>());
        Assert.DoesNotThrow(() => ServiceProvider.GetRequiredService<IMegVersionIdentifier>());
        Assert.DoesNotThrow(() => ServiceProvider.GetRequiredService<IMegDataStreamFactory>());
        Assert.DoesNotThrow(() => ServiceProvider.GetRequiredService<IVirtualMegArchiveBuilder>());
        Assert.DoesNotThrow(() => ServiceProvider.GetRequiredService<IDataEntryPathResolver>());
    }
}