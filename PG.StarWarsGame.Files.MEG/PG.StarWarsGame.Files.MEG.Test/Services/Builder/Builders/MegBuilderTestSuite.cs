using System;
using PG.StarWarsGame.Files.MEG.Services.Builder;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

public abstract class MegBuilderTestSuite : CommonMegTestBase
{
    protected abstract Type ExpectedFileInfoValidatorType { get; }
    protected abstract Type ExpectedDataEntryValidatorType { get; }
    protected abstract Type? ExpectedDataEntryPathNormalizerType { get; }
    protected abstract bool? ExpectedOverwritesDuplicates { get; }
    protected abstract bool? ExpectedAutomaticallyAddFileSizes { get; }

    protected abstract MegBuilderBase CreateBuilder();
    
    [Fact]
    public void MegBuilderTestSuite_Test_Ctor()
    {
        var builder = CreateBuilder();

        Assert.Equal(ExpectedFileInfoValidatorType, builder.MegFileInformationValidator.GetType());
        Assert.Equal(ExpectedDataEntryValidatorType, builder.DataEntryValidator.GetType());
        Assert.Equal(ExpectedDataEntryPathNormalizerType, builder.DataEntryPathNormalizer?.GetType());
        Assert.Equal(ExpectedOverwritesDuplicates, builder.OverwritesDuplicateEntries);
        Assert.Equal(ExpectedAutomaticallyAddFileSizes, builder.AutomaticallyAddFileSizes);
    }
}