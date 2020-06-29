using System.Collections.Generic;
using PG.StarWarsGame.Files.DAT.Holder;

namespace PG.StarWarsGame.Files.DAT.Services
{
    public interface ISortedDatFileProcessService
    {
        SortedDatFileHolder LoadFromFile(string filePath);
        void SaveToFile(SortedDatFileHolder sortedDatFileHolder);
    }
}
