using System;
using System.Collections.Generic;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services.Builder;
using PG.StarWarsGame.Files.Test.Services.Builder;
using Xunit;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

public abstract class MegBuilderTestBase<TBuilder> : FileBuilderTestBase<TBuilder, IReadOnlyCollection<MegFileDataEntryBuilderInfo>, MegFileInformation>
    where TBuilder : MegBuilderBase
{
    protected abstract Type ExpectedFileInfoValidatorType { get; }
    protected abstract Type ExpectedDataEntryValidatorType { get; }
    protected abstract Type? ExpectedDataEntryPathNormalizerType { get; }
    protected abstract bool? ExpectedOverwritesDuplicates { get; }
    protected abstract bool? ExpectedAutomaticallyAddFileSizes { get; }

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