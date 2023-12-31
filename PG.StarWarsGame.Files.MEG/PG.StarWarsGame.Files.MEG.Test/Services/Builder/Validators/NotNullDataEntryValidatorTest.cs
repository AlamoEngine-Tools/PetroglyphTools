using System.Collections.Generic;
using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Services.Builder;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

[TestClass]
public class NotNullDataEntryValidatorTest : ValidatorTestSuite<MegFileDataEntryBuilderInfo>
{
    protected override IValidator<MegFileDataEntryBuilderInfo> CreateValidator()
    {
        return MegBuilderBase.NotNullDataEntryValidator.Instance;
    }

    protected override IEnumerable<ValidatorTestData<MegFileDataEntryBuilderInfo>> GetValidCases()
    {
        yield return new ValidatorTestData<MegFileDataEntryBuilderInfo>(
            new MegFileDataEntryBuilderInfo(new MegDataEntryOriginInfo("path")), "Test_NotNull");
    }

    protected override IEnumerable<ValidatorTestData<MegFileDataEntryBuilderInfo>> GetInvalidCases()
    {
        yield return new ValidatorTestData<MegFileDataEntryBuilderInfo>(null!, "Test_Null");
    }
}