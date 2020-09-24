using System.Collections.Generic;
using JetBrains.Annotations;
using PG.StarWarsGame.Files.DAT.Holder;

namespace PG.StarWarsGame.Localisation.Data.Holder
{
    internal interface ILocalisationHolder<T> where T : ILocalisationElement
    {
        bool TryAdd([NotNull] string key, [NotNull] T value);
        bool TryUpdate([NotNull] string key, [NotNull] T value);
        T Get([NotNull] string key);
        IEnumerable<T> GetAll();
        IEnumerable<string> GetAllKeys();
        void Merge(SortedDatFileHolder holder);
    }
}