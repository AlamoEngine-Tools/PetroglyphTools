using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using PG.Commons.Util;
using PG.StarWarsGame.Files.DAT.Files;

[assembly: InternalsVisibleTo("PG.StarWarsGame.Files.DAT.Test")]

namespace PG.StarWarsGame.Files.DAT.Holder
{
    public sealed class SortedDatFileHolder : ADatFileHolder<List<Tuple<string, string>>, SortedDatAlamoFileType>
    {
        [NotNull] private readonly List<Tuple<string, string>> m_content = new List<Tuple<string, string>>();

        public SortedDatFileHolder(string filePath, string fileName) : base(filePath, fileName)
        {
        }

        public override SortedDatAlamoFileType FileType { get; } = new SortedDatAlamoFileType();

        public override List<Tuple<string, string>> Content
        {
            get
            {
                m_content.Sort((t1, t2) => ChecksumUtility.GetChecksum(t1?.Item1 ?? string.Empty)
                    .CompareTo(ChecksumUtility.GetChecksum(t2?.Item1 ?? string.Empty)));
                return m_content;
            }
            set
            {
                m_content.Clear();
                m_content.AddRange(value);
            }
        }
    }
}
