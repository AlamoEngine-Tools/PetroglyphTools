using System;
using System.Collections.Generic;
using System.Globalization;

namespace PG.StarWarsGame.Localisation.Data.Holder
{
    internal class LocalisationElement
    {
        private readonly Dictionary<CultureInfo, string> m_localisation;
        internal const string MISSING = "[MISSING]";

        internal LocalisationElement(Dictionary<CultureInfo, string> localisation)
        {
            m_localisation = localisation;
        }

        internal bool TryAdd(CultureInfo key, string value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            value ??= string.Empty;
            return m_localisation.TryAdd(key, value);
        }

        internal bool TryUpdate(CultureInfo key, string value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            value ??= string.Empty;
            if (!m_localisation.ContainsKey(key)) return TryAdd(key, value);
            m_localisation[key] = value;
            return true;
        }

        internal string Get(CultureInfo key)
        {
            return m_localisation.ContainsKey(key) ? m_localisation[key] : MISSING;
        }

        internal IEnumerable<string> GetAll()
        {
            return m_localisation.Values;
        }

        internal IEnumerable<CultureInfo> GetAllKeys()
        {
            return m_localisation.Keys;
        }
        
        internal static LocalisationElement GetEmptyElement()
        {
            return new LocalisationElement(new Dictionary<CultureInfo, string>());
        }
    }
}