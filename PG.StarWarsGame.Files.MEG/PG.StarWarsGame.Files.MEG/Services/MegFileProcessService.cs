using System.Composition;
using PG.StarWarsGame.Files.MEG.Holder;

namespace PG.StarWarsGame.Files.MEG.Services
{
    [Export(nameof(IMegFileProcessService))]
    internal class MegFileProcessService : IMegFileProcessService
    {
        //TODO [gruenwaldlu, 2020-06-30-21:41:03+2]: Implement!
        public void Unpack(MegFileHolder holder)
        {
            throw new System.NotImplementedException();
        }

        //TODO [gruenwaldlu, 2020-06-30-21:41:15+2]: Implement!
        public void Pack(MegFileHolder holder)
        {
            throw new System.NotImplementedException();
        }

        //TODO [gruenwaldlu, 2020-06-30-21:41:25+2]: Implement!
        public MegFileHolder Load(string filePath)
        {
            throw new System.NotImplementedException();
        }
    }
}