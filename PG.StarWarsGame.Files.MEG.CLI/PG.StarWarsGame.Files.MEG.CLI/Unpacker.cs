using System;

namespace PG.StarWarsGame.Files.MEG.CLI
{
    internal class Unpacker : IApplicationTask
    {
        public UnpackOptions Options { get; }

        public Unpacker(UnpackOptions options)
        {
            Options = options;
        }

        public void Run()
        {
            throw new NotImplementedException();
        }
    }
}