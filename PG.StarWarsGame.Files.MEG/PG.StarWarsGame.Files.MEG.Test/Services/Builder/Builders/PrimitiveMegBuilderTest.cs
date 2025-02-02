using System;
using System.Collections.Generic;
using System.Linq;
using PG.Commons.Hashing;
using PG.StarWarsGame.Files.Binary;
using PG.StarWarsGame.Files.MEG.Binary.Metadata;
using PG.StarWarsGame.Files.MEG.Binary.Metadata.V1;
using PG.StarWarsGame.Files.MEG.Data;
using PG.StarWarsGame.Files.MEG.Data.EntryLocations;
using PG.StarWarsGame.Files.MEG.Services.Builder;
using PG.StarWarsGame.Files.MEG.Services.Builder.Validation;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

public class PrimitiveMegBuilderTest : MegBuilderTestBase<PrimitiveMegBuilder>
{
    protected override Type ExpectedFileInfoValidatorType => typeof(DefaultMegFileInformationValidator);
    protected override Type ExpectedDataEntryValidatorType => typeof(NotNullDataEntryValidator);
    protected override Type? ExpectedDataEntryPathNormalizerType => null;
    protected override bool? ExpectedOverwritesDuplicates => true;
    protected override bool? ExpectedAutomaticallyAddFileSizes => false;

    protected override bool FileInfoIsAlwaysValid => true;

    protected override PrimitiveMegBuilder CreateBuilder()
    {
        return new PrimitiveMegBuilder(ServiceProvider);
    }

    protected override void AddDataToBuilder(IReadOnlyCollection<MegFileDataEntryBuilderInfo> data, PrimitiveMegBuilder builder)
    {
        foreach (var info in data)
        {
            builder.AddFile(info.FilePath, info.FilePath);
        }
    }

    protected override (IReadOnlyCollection<MegFileDataEntryBuilderInfo> Data, byte[] Bytes) CreateValidData()
    {
        var oneBytes = new byte[] { 1, 2, 3, 4, 5, 6 };
        var twoBytes = new byte[] { 6, 5, 4, 3, 2, 1 };
        FileSystem.File.WriteAllBytes("1.txt", oneBytes);
        FileSystem.File.WriteAllBytes("2.txt", twoBytes);

        var testMeg = new List<MegFileDataEntryBuilderInfo>
        {
            new(new MegDataEntryOriginInfo("1.txt"), "1.txt"),
            new(new MegDataEntryOriginInfo("2.txt"), "2.txt"),
        };


        var header = new MegHeader(2, 2);
        var nameTable = new BinaryTable<MegFileNameTableRecord>([
            new MegFileNameTableRecord("2.txt", "2.txt"), // entry name is NOT upper-case
            new MegFileNameTableRecord("1.txt", "1.txt"), // "2.txt" has lower CRC than "1.txt"
        ]);


        var megBin = new MegMetadata(header, nameTable,
            new MegFileTable([
                new MegFileTableRecord(new Crc32(3193455265),
                    0,
                    6,
                    (uint)(header.Size + nameTable.Size + 2 * 20 + 0),
                    0),
                new MegFileTableRecord(
                    new Crc32(4193794161),
                    1,
                    6,
                    (uint)(header.Size + nameTable.Size + 2 * 20 + 6),
                    1),
            ]));

        return (testMeg, megBin.Bytes.Concat(twoBytes).Concat(oneBytes).ToArray());
    }
}