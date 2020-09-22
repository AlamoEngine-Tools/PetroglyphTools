using System;
using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using PG.StarWarsGame.Files.DAT.Holder;
using PG.StarWarsGame.Files.DAT.Services;

namespace PG.StarWarsGame.Localisation.Data.Holder
{
    internal class LocalisationHolder
    {
        private readonly Dictionary<string, ILocalisationElement> m_localisationElements;
        private readonly ILogger<LocalisationHolder> m_logger;
        private readonly IDatFileUtilityService m_datFileUtilityService;

        public LocalisationHolder(IDatFileUtilityService datFileUtilityService,
            ILoggerFactory loggerFactory = null)
        {
            m_localisationElements = new Dictionary<string, ILocalisationElement>();
            m_datFileUtilityService = datFileUtilityService;
            m_logger = loggerFactory?.CreateLogger<LocalisationHolder>();
        }

        internal bool TryAdd([NotNull] string key, [NotNull] ILocalisationElement value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            return m_localisationElements.TryAdd(key, value);
        }

        internal bool TryUpdate([NotNull] string key, [NotNull] ILocalisationElement value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (!m_localisationElements.ContainsKey(key)) return TryAdd(key, value);
            m_localisationElements[key] = value;
            return true;
        }

        internal ILocalisationElement Get([NotNull] string key)
        {
            return m_localisationElements.ContainsKey(key)
                ? m_localisationElements[key]
                : null;
        }

        internal IEnumerable<ILocalisationElement> GetAll()
        {
            return m_localisationElements.Values;
        }

        internal IEnumerable<string> GetAllKeys()
        {
            return m_localisationElements.Keys;
        }

        internal void Merge(SortedDatFileHolder holder)
        {
            if (!m_datFileUtilityService.TryGetCultureInfoFromFileName(holder.FileName, out CultureInfo culture))
            {
                culture = m_datFileUtilityService.ConfiguredDefaultCultureInfo;
            }

            foreach ((string key, string value) in holder.Content)
            {
                try
                {
                    if (m_localisationElements.ContainsKey(key))
                    {
                        if (!m_localisationElements[key].TryUpdate(culture, value))
                        {
                            m_logger?.LogWarning(
                                $"Could not merge item \"{key}\"=\"{value}\" for culture \"{culture.Name}\"");
                        }
                    }

                    LocalisationElement element = LocalisationElement.GetEmptyElement();
                    element.TryUpdate(culture, value);
                    TryAdd(key, element);
                }
                catch (Exception e)
                {
                    m_logger?.LogError(
                        $"An error occured when attempting to merge the localisation item \"{key}\"=\"{value}\" for culture \"{culture.Name}\" into the existing database.",
                        e);
                }
            }
        }

        internal static LocalisationHolder GetEmptyDefault(IDatFileUtilityService datFileUtilityService,
            ILoggerFactory loggerFactory = null)
        {
            return new LocalisationHolder(datFileUtilityService, loggerFactory);
        }
    }
}