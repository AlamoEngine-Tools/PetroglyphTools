using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using PG.StarWarsGame.Files.DAT.Holder;
using PG.StarWarsGame.Files.DAT.Services;

namespace PG.StarWarsGame.Localisation.Data.Holder.Impl
{
    internal abstract class ALocalisationHolder<T> : ILocalisationHolder<T> where T : ILocalisationElement
    {
        protected readonly Dictionary<string, T> LocalisationElements;
        protected readonly IDatFileUtilityService DatFileUtilityService;

        protected ALocalisationHolder(IDatFileUtilityService datFileUtilityService)
        {
            LocalisationElements = new Dictionary<string, T>();
            DatFileUtilityService = datFileUtilityService;
        }

        public bool TryAdd([NotNull] string key, [NotNull] T value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            return LocalisationElements.TryAdd(key, value);
        }

        public bool TryUpdate([NotNull] string key, [NotNull] T value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (!LocalisationElements.ContainsKey(key)) return TryAdd(key, value);
            LocalisationElements[key] = value;
            return true;
        }

        public abstract T Get([NotNull] string key);

        public IEnumerable<T> GetAll()
        {
            return LocalisationElements.Values;
        }

        public IEnumerable<string> GetAllKeys()
        {
            return LocalisationElements.Keys;
        }

        public abstract void Merge(SortedDatFileHolder holder);
    }
}