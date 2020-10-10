using System;

namespace PG.StarWarsGame.Files.MEG.CLI
{
    internal class Packer : IApplicationTask
    {
        public PackOptions Options { get; }

        public Packer(PackOptions options)
        {
            Options = options;
        }

        public void Run()
        {
            throw new NotImplementedException();
        }
    }
}