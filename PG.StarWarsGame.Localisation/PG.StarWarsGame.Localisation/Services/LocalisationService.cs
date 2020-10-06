using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Composition;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using CsvHelper;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using PG.Commons.Exceptions;
using PG.Commons.Util;
using PG.StarWarsGame.Files.DAT.Holder;
using PG.StarWarsGame.Files.DAT.Services;
using PG.StarWarsGame.Localisation.Commons.Exceptions;
using PG.StarWarsGame.Localisation.Data.Config;
using PG.StarWarsGame.Localisation.Data.Config.EaWTextEditorXml;
using PG.StarWarsGame.Localisation.Data.Holder;

namespace PG.StarWarsGame.Localisation.Services
{
    /// <summary>
    /// Default implementation for <see cref="ILocalisationService"/>
    /// </summary>
    [Export(nameof(ILocalisationService))]
    public class LocalisationService : ILocalisationService
    {
        private const string MASTER_TEXT_FILE_BASE_NAME_BUILDER_PART = "MASTERTEXTFILE_";
        private const string EAW_TEXT_EDITOR_PROJECT_FILE_NAME = "TranslationManifest.xml";

        [CanBeNull] private readonly ILogger m_logger;
        [NotNull] private readonly IFileSystem m_fileSystem;
        [NotNull] private readonly IDatFileUtilityService m_datFileUtilityService;
        [NotNull] private readonly ISortedDatFileProcessService m_sortedDatFileProcessService;
        [NotNull] private readonly ReadOnlyDictionary<string, LocalisationElement> m_core;
        [NotNull] private readonly ReadOnlyDictionary<string, LocalisationElement> m_expansion;
        [NotNull] private readonly Dictionary<string, LocalisationElement> m_mod;

        private bool m_loaded = false;

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="fileSystem">Required dependency.</param>
        /// <param name="datFileUtilityService">Required dependency.</param>
        /// <param name="sortedDatFileProcessService">Required dependency.</param>
        /// <param name="loggerFactory">Optional dependency.</param>
        public LocalisationService(IFileSystem fileSystem,
            [NotNull] IDatFileUtilityService datFileUtilityService,
            [NotNull] ISortedDatFileProcessService sortedDatFileProcessService,
            [CanBeNull] ILoggerFactory loggerFactory = null)
        {
            m_logger = loggerFactory?.CreateLogger<LocalisationService>();
            m_fileSystem = fileSystem ?? new FileSystem();
            m_datFileUtilityService =
                datFileUtilityService ?? throw new ArgumentNullException(nameof(datFileUtilityService));
            m_sortedDatFileProcessService = sortedDatFileProcessService ??
                                            throw new ArgumentNullException(nameof(sortedDatFileProcessService));
            m_core = AssembleReadonlyCore();
            m_expansion = AssembleReadonlyExpansion();
            m_mod = new Dictionary<string, LocalisationElement>();
        }

        private static ReadOnlyDictionary<string, LocalisationElement> AssembleReadonlyCore()
        {
            Dictionary<string, LocalisationElement> core = new Dictionary<string, LocalisationElement>();
            
            return new ReadOnlyDictionary<string, LocalisationElement>(core);
        }

        private static ReadOnlyDictionary<string, LocalisationElement> AssembleReadonlyExpansion()
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc/>
        public void LoadLocalisationProject(string textProjectDirectoryPath, ConfigVersion textProjectConfigVersion)
        {
            switch (textProjectConfigVersion)
            {
                case ConfigVersion.Invalid:
                    throw new InvalidVersionException(
                        $"The targeted version {nameof(ConfigVersion.Invalid)} is not valid in this context.");
                case ConfigVersion.EaWTextEditorXml:
                    throw new System.NotImplementedException();
                case ConfigVersion.HierarchicalTextProject:
                    throw new System.NotImplementedException();
                case ConfigVersion.SingleFileCsv:
                    throw new System.NotImplementedException();
                case ConfigVersion.DatFiles:
                    throw new System.NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException(nameof(textProjectConfigVersion), textProjectConfigVersion,
                        string.Empty);
            }
        }

        ///<inheritdoc/>
        public string GetLocalisation(string textKey, CultureInfo cultureInfo)
        {
            if (!m_loaded)
                throw new LocalisationProjectNotLoadedException(
                    "The localisation service has been created, but no localisation project has been loaded.");
            if (m_mod.ContainsKey(textKey))
            {
                return m_mod[textKey].Get(cultureInfo);
            }

            if (m_expansion.ContainsKey(textKey))
            {
                return m_expansion[textKey].Get(cultureInfo);
            }

            return m_core.ContainsKey(textKey) ? m_core[textKey].Get(cultureInfo) : LocalisationElement.MISSING;
        }

        private LocalisationElement GetLocalisationElement(string textKey)
        {
            if (!m_loaded)
                throw new LocalisationProjectNotLoadedException(
                    "The localisation service has been created, but no localisation project has been loaded.");
            if (m_mod.ContainsKey(textKey))
            {
                return m_mod[textKey];
            }

            if (m_expansion.ContainsKey(textKey))
            {
                return m_expansion[textKey];
            }
            
            return m_core.ContainsKey(textKey) ? m_core[textKey] : LocalisationElement.GetEmptyElement();
        }

        ///<inheritdoc/>
        public IEnumerable<string> GetAllLocalisations(string textKey)
        {
            if (!m_loaded)
                throw new LocalisationProjectNotLoadedException(
                    "The localisation service has been created, but no localisation project has been loaded.");
            if (m_mod.ContainsKey(textKey))
            {
                return m_mod[textKey].GetAll();
            }

            if (m_expansion.ContainsKey(textKey))
            {
                return m_expansion[textKey].GetAll();
            }

            return m_core.ContainsKey(textKey) ? m_core[textKey].GetAll() : new List<string>();
        }

        ///<inheritdoc/>
        public bool TryUpdateLocalisation(string textKey, string localisation, CultureInfo cultureInfo)
        {
            if (!m_loaded)
                throw new LocalisationProjectNotLoadedException(
                    "The localisation service has been created, but no localisation project has been loaded.");
            LocalisationElement element;
            if (m_mod.ContainsKey(textKey))
            {
                element = m_mod[textKey];
            }
            else
            {
                if (m_expansion.ContainsKey(textKey))
                {
                    element = LocalisationElement.GetEmptyElement();
                    // [gruenwaldlu]: If an element is overwritten in the hierarchy, it has to be overwritten in ALL languages. 
                    IEnumerable<CultureInfo> cultures = CollectAllCulturesFromProject();
                    foreach (CultureInfo culture in cultures)
                    {
                        element.TryUpdate(culture, LocalisationElement.MISSING);
                    }
                    m_mod.Add(textKey, element);
                }
                else
                {
                    if (!m_core.ContainsKey(textKey))
                    {
                        return false;
                    }

                    element = LocalisationElement.GetEmptyElement();
                    // [gruenwaldlu]: If an element is overwritten in the hierarchy, it has to be overwritten in ALL languages.
                    IEnumerable<CultureInfo> cultures = CollectAllCulturesFromProject();
                    foreach (CultureInfo culture in cultures)
                    {
                        element.TryUpdate(culture, LocalisationElement.MISSING);
                    }
                    m_mod.Add(textKey, element);
                }
            }

            return element != null && element.TryUpdate(cultureInfo, localisation);
        }

        ///<inheritdoc/>
        public bool TryAddLocalisation(string textKey, string localisation, CultureInfo cultureInfo)
        {
            if (!m_loaded)
                throw new LocalisationProjectNotLoadedException(
                    "The localisation service has been created, but no localisation project has been loaded.");
            if (TryUpdateLocalisation(textKey, localisation, cultureInfo)) return true;
            LocalisationElement element = LocalisationElement.GetEmptyElement();
            IEnumerable<CultureInfo> cultures = CollectAllCulturesFromProject();
            foreach (CultureInfo culture in cultures)
            {
                element.TryUpdate(culture, LocalisationElement.MISSING);
            }

            if (!element.TryUpdate(cultureInfo, localisation)) return false;
            m_mod.Add(textKey, element);
            return true;

        }

        ///<inheritdoc/>
        public void SaveToDisc(string textProjectDirectoryPath, ConfigVersion textProjectConfigVersion,
            bool cleanDirectory = true)
        {
            if (!m_loaded)
                throw new LocalisationProjectNotLoadedException(
                    "The localisation service has been created, but no localisation project has been loaded.");
            switch (textProjectConfigVersion)
            {
                case ConfigVersion.Invalid:
                    throw new InvalidVersionException(
                        $"The targeted version {nameof(ConfigVersion.Invalid)} is not valid in this context.");
                case ConfigVersion.EaWTextEditorXml:
                    SaveEaWTextEditorXmlToDiscInternal(textProjectDirectoryPath);
                    break;
                case ConfigVersion.HierarchicalTextProject:
                    SaveHierarchicalTextProjectToDisc(textProjectDirectoryPath);
                    break;
                case ConfigVersion.SingleFileCsv:
                    SaveSingleFileCsvToDiscInternal(textProjectDirectoryPath);
                    break;
                case ConfigVersion.DatFiles:
                    SaveDatFilesToDiscInternal(textProjectDirectoryPath);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(textProjectConfigVersion), textProjectConfigVersion,
                        string.Empty);
            }
        }

        private void SaveHierarchicalTextProjectToDisc(string textProjectDirectoryPath)
        {
            throw new NotImplementedException();
        }

        private void SaveSingleFileCsvToDiscInternal(string textProjectDirectoryPath)
        {
            IEnumerable<CultureInfo> cultures = CollectAllCulturesFromProject();
            foreach (CultureInfo cultureInfo in cultures)
            {
                if (m_datFileUtilityService.TryGetFileNamePartFromCultureInfo(cultureInfo, out string languageIndicator)
                )
                {
                    IEnumerable<Data.Config.Csv.Localisation> records = CreateCsvRecordsForSingleFileCsv(cultureInfo);
                    string exportPath = m_fileSystem.Path.Combine(textProjectDirectoryPath,
                        MASTER_TEXT_FILE_BASE_NAME_BUILDER_PART + languageIndicator + ".csv");
                    using StreamWriter writer =
                        new StreamWriter(m_fileSystem.File.Open(exportPath, FileMode.OpenOrCreate));
                    using CsvWriter csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
                    csvWriter.WriteRecords(records);
                }
                else
                {
                    m_logger.LogError(
                        $"The text project contains the unsupported culture \"{cultureInfo}\". CSV file creation will be skipped for this culture.\nIf you want to see your language supported in future versions of the library, please open an issue at https://github.com/AlamoEngine-Tools/PetroglyphTools/issues providing the language and the expected MasterTextFile name.");
                }
            }
        }

        private IEnumerable<Data.Config.Csv.Localisation> CreateCsvRecordsForSingleFileCsv(CultureInfo cultureInfo)
        {
            List<Data.Config.Csv.Localisation> records = new List<Data.Config.Csv.Localisation>();
            foreach (string key in CollectAllKeysFromProject())
            {
                try
                {
                    records.Add(new Data.Config.Csv.Localisation
                    {
                        Key = key,
                        Text = GetLocalisation(key, cultureInfo)
                    });
                }
                catch (Exception e)
                {
                    m_logger?.LogWarning(
                        $"The CSV-record for  key \"{key}\" with culture \"{cultureInfo}\" could not be created. Skipping.",
                        e);
                }
            }

            return records;
        }

        private void SaveEaWTextEditorXmlToDiscInternal(string textProjectPath)
        {
            List<LocalisationType> localisations = new List<LocalisationType>();
            foreach (string key in CollectAllKeysFromProject())
            {
                List<TranslationType> translations = new List<TranslationType>();
                LocalisationElement localisationElement = GetLocalisationElement(key);
                foreach (CultureInfo cultureInfo in localisationElement.GetAllKeys())
                {
                    if (m_datFileUtilityService.TryGetFileNamePartFromCultureInfo(cultureInfo, out string fileNamePart))
                    {
                        translations.Add(new TranslationType
                        {
                            Language = fileNamePart,
                            Value = localisationElement.Get(cultureInfo)
                        });
                    }
                    else
                    {
                        m_logger?.LogWarning(
                            $"The translation for the language \"{cultureInfo}\" could not be created. Skipping.");
                    }
                }

                localisations.Add(new LocalisationType
                {
                    Key = key, TranslationData = translations.ToArray()
                });
            }

            LocalisationDataType xml = new LocalisationDataType {Localisation = localisations.ToArray()};
            string targetPath = textProjectPath;
            if (!m_fileSystem.Path.HasExtension(textProjectPath))
            {
                targetPath = m_fileSystem.Path.Combine(textProjectPath, EAW_TEXT_EDITOR_PROJECT_FILE_NAME);
            }

            XmlUtility.WriteXmlFile(targetPath, xml);
        }

        private void SaveDatFilesToDiscInternal(string textProjectPath)
        {
            IEnumerable<SortedDatFileHolder> datFileHolders = GetDatFileHolders(textProjectPath);
            foreach (SortedDatFileHolder sortedDatFileHolder in datFileHolders)
            {
                try
                {
                    m_sortedDatFileProcessService.SaveToFile(sortedDatFileHolder);
                }
                catch (Exception e)
                {
                    m_logger?.LogError(
                        $"An error occurred whilst writing the file \"{sortedDatFileHolder.FilePath}{m_fileSystem.Path.DirectorySeparatorChar}{sortedDatFileHolder.FileName}.{sortedDatFileHolder.FileType.FileExtension}\" to disc.",
                        e);
                }
            }
        }

        private IEnumerable<CultureInfo> CollectAllCulturesFromProject()
        {
            HashSet<string> set = new HashSet<string>();
            List<CultureInfo> cultures = new List<CultureInfo>();
            foreach (LocalisationElement localisationElement in m_core.Values)
            {
                foreach (CultureInfo cultureInfo in localisationElement.GetAllKeys())
                {
                    if (set.Contains(cultureInfo.Name)) continue;
                    m_logger.LogInformation($"Discovered new culture info \"{cultureInfo}\".");
                    set.Add(cultureInfo.Name);
                    cultures.Add(cultureInfo);
                }
            }
            foreach (LocalisationElement localisationElement in m_expansion.Values)
            {
                foreach (CultureInfo cultureInfo in localisationElement.GetAllKeys())
                {
                    if (set.Contains(cultureInfo.Name)) continue;
                    m_logger.LogInformation($"Discovered new culture info \"{cultureInfo}\".");
                    set.Add(cultureInfo.Name);
                    cultures.Add(cultureInfo);
                }
            }
            foreach (LocalisationElement localisationElement in m_mod.Values)
            {
                foreach (CultureInfo cultureInfo in localisationElement.GetAllKeys())
                {
                    if (set.Contains(cultureInfo.Name)) continue;
                    m_logger.LogInformation($"Discovered new culture info \"{cultureInfo}\".");
                    set.Add(cultureInfo.Name);
                    cultures.Add(cultureInfo);
                }
            }
            return cultures;
        }

        private IEnumerable<SortedDatFileHolder> GetDatFileHolders(string textProjectPath)
        {
            List<SortedDatFileHolder> sortedDatFileHolders = new List<SortedDatFileHolder>();
            IEnumerable<CultureInfo> cultures = CollectAllCulturesFromProject();

            foreach (CultureInfo cultureInfo in cultures)
            {
                if (m_datFileUtilityService.TryGetFileNamePartFromCultureInfo(cultureInfo, out string languageIndicator)
                )
                {
                    IEnumerable<string> keys = CollectAllKeysFromProject();
                    List<Tuple<string, string>> initList =
                        (from key in keys
                            let localisation = GetLocalisation(key, cultureInfo)
                            where !string.IsNullOrEmpty(localisation)
                            select new Tuple<string, string>(key, localisation)).ToList();
                    SortedDatFileHolder sortedDatFileHolder =
                        new SortedDatFileHolder(textProjectPath,
                            MASTER_TEXT_FILE_BASE_NAME_BUILDER_PART + languageIndicator) {Content = initList};
                    sortedDatFileHolders.Add(sortedDatFileHolder);
                }
                else
                {
                    m_logger.LogError(
                        $"The text project contains the unsupported culture \"{cultureInfo}\". Dat file creation will be skipped for this culture.\nIf you want to see your language supported in future versions of the library, please open an issue at https://github.com/AlamoEngine-Tools/PetroglyphTools/issues providing the language and the expected MasterTextFile name.");
                }
            }

            return sortedDatFileHolders;
        }

        private IEnumerable<string> CollectAllKeysFromProject()
        {
            HashSet<string> uniqueKeys = new HashSet<string>();
            foreach (string coreKey in m_core.Keys)
            {
                uniqueKeys.Add(coreKey);
            }
            foreach (string expansionKey in m_expansion.Keys)
            {
                uniqueKeys.Add(expansionKey);
            }
            foreach (string modKey in m_mod.Keys)
            {
                uniqueKeys.Add(modKey);
            }
            return uniqueKeys.AsEnumerable();
        }
    }
}