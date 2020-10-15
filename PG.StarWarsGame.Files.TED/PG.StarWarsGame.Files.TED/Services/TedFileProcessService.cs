using System.Composition;
using PG.StarWarsGame.Files.TED.Holder;

namespace PG.StarWarsGame.Files.TED.Services
{
    [Export(nameof(ITedFileProcessService))]
    public class TedFileProcessService : ITedFileProcessService 
    {
        public TedFileHolder Load(string filePath)
        {
            throw new System.NotImplementedException();
        }

        public void Save(TedFileHolder mapFile)
        {
            throw new System.NotImplementedException();
        }
    }
}