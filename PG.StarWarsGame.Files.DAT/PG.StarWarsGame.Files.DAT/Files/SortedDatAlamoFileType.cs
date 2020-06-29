using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.DAT.Test")]

namespace PG.StarWarsGame.Files.DAT.Files
{
    public sealed class SortedDatAlamoFileType : ADatAlamoFileType
    {
        private const bool IS_SORTED = true;

        public override bool IsSorted => IS_SORTED;
    }
}
