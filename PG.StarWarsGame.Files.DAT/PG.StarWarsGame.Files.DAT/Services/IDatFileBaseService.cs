using PG.StarWarsGame.Files.DAT.Binary;
using PG.StarWarsGame.Files.DAT.Holder;

namespace PG.StarWarsGame.Files.DAT.Services
{
    public interface IDatFileBaseService
    {
        DatFile LoadFlat(string filePath);
        void StoreFlat(string filePath, SortedDatFileHolder fileHolder);
        void StoreFlat(string filePath, UnsortedDatFileHolder fileHolder);
    }
}
