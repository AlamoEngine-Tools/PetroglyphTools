using PG.StarWarsGame.Files.DAT.Holder;

namespace PG.StarWarsGame.Files.DAT.Services
{
    public interface IUnsortedDatFileProcessService
    {
        UnsortedDatFileHolder LoadFromFile(string filePath);
        void SaveToFile(UnsortedDatFileHolder unsortedDatFileHolder);
    }
}