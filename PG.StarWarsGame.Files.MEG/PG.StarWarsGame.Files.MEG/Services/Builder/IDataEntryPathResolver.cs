namespace PG.StarWarsGame.Files.MEG.Services.Builder;

internal interface IDataEntryPathResolver
{
    string? ResolvePath(string path, string basePath);
}