using System;
using System.Globalization;
using Microsoft.Extensions.Logging;
using PG.StarWarsGame.Files.DAT.Holder;
using PG.StarWarsGame.Files.DAT.Services;

namespace PG.StarWarsGame.Localisation.Data.Holder.Impl
{
    internal class HierarchicalLocalisationHolder : ALocalisationHolder<HierarchicalLocalisationElement>
    {
        private readonly ILogger<HierarchicalLocalisationHolder> m_logger;

        internal HierarchicalLocalisationHolder(IDatFileUtilityService datFileUtilityService,
            ILoggerFactory loggerFactory = null) : base(datFileUtilityService)
        {
            m_logger = loggerFactory?.CreateLogger<HierarchicalLocalisationHolder>();
        }

        public override HierarchicalLocalisationElement Get(string key)
        {
            return LocalisationElements.ContainsKey(key)
                ? LocalisationElements[key]
                : null;
        }

        public override void Merge(SortedDatFileHolder holder)
        {
            if (!DatFileUtilityService.TryGetCultureInfoFromFileName(holder.FileName, out CultureInfo culture))
            {
                culture = DatFileUtilityService.ConfiguredDefaultCultureInfo;
            }

            foreach ((string key, string value) in holder.Content)
            {
                try
                {
                    if (LocalisationElements.ContainsKey(key) && !LocalisationElements[key].TryUpdate(culture, value))
                    {
                        m_logger?.LogWarning(
                            $"Could not merge item \"{key}\"=\"{value}\" for culture \"{culture.Name}\"");
                    }


                    HierarchicalLocalisationElement element = HierarchicalLocalisationElement.GetEmptyElement();
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
    }
}