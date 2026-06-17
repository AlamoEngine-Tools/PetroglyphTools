using System.IO;
using Microsoft.Extensions.DependencyInjection;
using PG.StarWarsGame.Files.MEG.Data.Archives;
using PG.StarWarsGame.Files.MEG.Files;
using PG.StarWarsGame.Files.MEG.Services;
using Testably.Abstractions.Testing;
using Xunit;
using static PG.StarWarsGame.Files.MEG.Test.Data.Entries.MegDataEntryTest;

namespace PG.StarWarsGame.Files.MEG.Test.Services;

public class FileMegDataSourceTest : MegDataSourceTestSuite
{
    protected override IMegDataSource CreateMegDataSource(byte[] megBytes)
    {
        FileSystem.File.WriteAllBytes("test.meg", megBytes);
        return ServiceProvider.GetRequiredService<IMegFileService>().Load("test.meg");
    }

    [Fact]
    public void GetData_FileCannotBeRead_Throws()
    {
        var entry = CreateEntry("file.txt", default, 0, 12);

        _ = FileSystem.File.Create("test.meg"); // intentionally left open so the file is locked
        var meg = new MegFile(new MegArchive([entry]), new MegFileInformation("test.meg", MegFileVersion.V1), ServiceProvider);

        Assert.Throws<IOException>(() => meg.GetData(entry));
    }

    [Fact]
    public void GetData_EmptyEntry_BackingFileMissing_Throws()
    {
        // A zero-size entry whose backing file is gone still fails.
        FileSystem.Initialize().WithFile("test.meg");
        var entry = CreateEntry("file.txt", offset: 2, size: 0);
        var meg = new MegFile(new MegArchive([entry]), new MegFileInformation("test.meg", MegFileVersion.V1), ServiceProvider);

        FileSystem.File.Delete("test.meg");

        Assert.Throws<FileNotFoundException>(() => meg.GetData(entry));
    }
}

public class InMemoryMegDataSourceTest : MegDataSourceTestSuite
{
    protected override IMegDataSource CreateMegDataSource(byte[] megBytes)
    {
        return ServiceProvider.GetRequiredService<IMegService>().LoadArchive(megBytes);
    }
}
