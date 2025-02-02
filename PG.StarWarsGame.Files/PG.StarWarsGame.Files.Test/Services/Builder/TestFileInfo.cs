namespace PG.StarWarsGame.Files.Test.Services.Builder;

public record TestFileInfo : PetroglyphFileInformation
{
    public bool IsValid { get; init; }

    public static TestFileInfo Create(string path, bool valid)
    {
        return new TestFileInfo
        {
            FilePath = path,
            IsValid = valid
        };
    }
}