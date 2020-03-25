using PG.StarWarsGame.Files.DAT.Holder;

namespace PG.StarWarsGame.Files.DAT.Services
{
    public interface IDatFileProcessService
    {
        SortedDatFileHolder LoadSorted(string filePath);
        UnsortedDatFileHolder LoadUnsorted(string filePath);
        void Save(SortedDatFileHolder fileHolder);
        void Save(UnsortedDatFileHolder fileHolder);
    }
}
