using System;
using System.Collections.Generic;
using System.Globalization;

namespace PG.StarWarsGame.Localisation.Data.Holder.Impl
{
    internal abstract class ALocalisationElement : ILocalisationElement
    {
        private readonly Dictionary<CultureInfo, string> m_localisation;
        private const string MISSING = "[MISSING]";

        protected ALocalisationElement(Dictionary<CultureInfo, string> localisation)
        {
            m_localisation = localisation;
        }

        public bool TryAdd(CultureInfo key, string value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            value ??= string.Empty;
            return m_localisation.TryAdd(key, value);
        }

        public bool TryUpdate(CultureInfo key, string value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            value ??= string.Empty;
            if (!m_localisation.ContainsKey(key)) return TryAdd(key, value);
            m_localisation[key] = value;
            return true;
        }

        public string Get(CultureInfo key)
        {
            return m_localisation.ContainsKey(key) ? m_localisation[key] : MISSING;
        }

        public IEnumerable<string> GetAll()
        {
            return m_localisation.Values;
        }

        public IEnumerable<CultureInfo> GetAllKeys()
        {
            return m_localisation.Keys;
        }
    }
}