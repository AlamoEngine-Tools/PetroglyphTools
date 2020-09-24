using System;
using System.Collections.Generic;
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
        private ILocalisationHolder<ILocalisationElement> m_localisationHolder;

        private bool Loaded { get; set; }

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
            if (!Loaded)
                throw new LocalisationProjectNotLoadedException(
                    "The localisation service has been created, but no localisation project has been loaded.");
            return m_localisationHolder.Get(textKey).Get(cultureInfo);
        }

        ///<inheritdoc/>
        public IEnumerable<string> GetAllLocalisations(string textKey)
        {
            if (!Loaded)
                throw new LocalisationProjectNotLoadedException(
                    "The localisation service has been created, but no localisation project has been loaded.");
            return m_localisationHolder.Get(textKey).GetAll();
        }

        ///<inheritdoc/>
        public bool TryUpdateLocalisation(string textKey, string localisation, CultureInfo cultureInfo)
        {
            if (!Loaded)
                throw new LocalisationProjectNotLoadedException(
                    "The localisation service has been created, but no localisation project has been loaded.");
            ILocalisationElement element = m_localisationHolder.Get(textKey);
            return element != null && element.TryUpdate(cultureInfo, localisation);
        }

        ///<inheritdoc/>
        public bool TryAddLocalisation(string textKey, string localisation, CultureInfo cultureInfo)
        {
            if (!Loaded)
                throw new LocalisationProjectNotLoadedException(
                    "The localisation service has been created, but no localisation project has been loaded.");
            throw new System.NotImplementedException();
        }

        ///<inheritdoc/>
        public void SaveToDisc(string textProjectDirectoryPath, ConfigVersion textProjectConfigVersion,
            bool cleanDirectory = true)
        {
            if (!Loaded)
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
                    throw new System.NotImplementedException();
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

        private void SaveSingleFileCsvToDiscInternal(string textProjectDirectoryPath)
        {
            IEnumerable<CultureInfo> cultures = CollectAllCulturesFromProject();
            foreach (CultureInfo cultureInfo in cultures)
            {
                if (m_datFileUtilityService.TryGetFileNamePartFromCultureInfo(cultureInfo, out string languageIndicator)
                )
                {
                    IEnumerable<Data.Config.Csv.Localisation> records = CreateCsvRecords(cultureInfo);
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

        private IEnumerable<Data.Config.Csv.Localisation> CreateCsvRecords(CultureInfo cultureInfo)
        {
            List<Data.Config.Csv.Localisation> records = new List<Data.Config.Csv.Localisation>();
            foreach (string key in m_localisationHolder.GetAllKeys())
            {
                try
                {
                    records.Add(new Data.Config.Csv.Localisation
                    {
                        Key = key,
                        Text = m_localisationHolder.Get(key).Get(cultureInfo)
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
            foreach (string key in m_localisationHolder.GetAllKeys())
            {
                List<TranslationType> translations = new List<TranslationType>();
                ILocalisationElement localisationElement = m_localisationHolder.Get(key);
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
            foreach (ILocalisationElement localisationElement in m_localisationHolder.GetAll())
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
                    IEnumerable<string> keys = m_localisationHolder.GetAllKeys();
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
    }
}