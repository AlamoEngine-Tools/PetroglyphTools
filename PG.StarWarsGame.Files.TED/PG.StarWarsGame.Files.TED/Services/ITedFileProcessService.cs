using PG.StarWarsGame.Files.TED.Holder;

namespace PG.StarWarsGame.Files.TED.Services
{
    public interface ITedFileProcessService
    {
        TedFileHolder Load(string filePath);

        void Save(TedFileHolder mapFile);
    }
}
