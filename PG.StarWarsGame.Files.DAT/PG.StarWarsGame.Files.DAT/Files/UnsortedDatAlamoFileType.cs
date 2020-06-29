using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.DAT.Test")]

namespace PG.StarWarsGame.Files.DAT.Files
{
    public sealed class UnsortedDatAlamoFileType : ADatAlamoFileType
    {
        private const bool IS_SORTED = false;

        public override bool IsSorted => IS_SORTED;
    }
}
