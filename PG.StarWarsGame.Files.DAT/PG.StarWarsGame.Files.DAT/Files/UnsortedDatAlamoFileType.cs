using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.DAT.Test")]

namespace PG.StarWarsGame.Files.DAT.Files
{
    /// <summary>
    /// The unsorted file type implementation of <see cref="PG.Commons.Data.Files.IAlamoFileType"/> is used as a credits
    /// file (yes, like those movie credits). It allows duplicate keys which are used as formatting instructions and the
    /// given order of keys is retained.
    /// </summary>
    public sealed class UnsortedDatAlamoFileType : ADatAlamoFileType
    {
        private const bool IS_SORTED = false;

        public override bool IsSorted => IS_SORTED;
    }
}
