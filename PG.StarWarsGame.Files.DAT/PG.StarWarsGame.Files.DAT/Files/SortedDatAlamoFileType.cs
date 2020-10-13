using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.DAT.Test")]

namespace PG.StarWarsGame.Files.DAT.Files
{
    /// <summary>
    /// The sorted file type implementation of <see cref="PG.Commons.Data.Files.IAlamoFileType"/>.
    /// does not allow duplicate keys and its contents are sorted ascending by the key's custom CRC32 <see cref="PG.Commons.Util.ChecksumUtility"/>.
    /// It is used for all localisation purposes.
    /// </summary>
    public sealed class SortedDatAlamoFileType : ADatAlamoFileType
    {
        private const bool IS_SORTED = true;

        public override bool IsSorted => IS_SORTED;
    }
}
