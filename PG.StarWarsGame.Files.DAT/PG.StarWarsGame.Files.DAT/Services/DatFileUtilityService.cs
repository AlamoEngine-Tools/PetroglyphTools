using System;
using System.Composition;
using System.Globalization;
using Microsoft.Extensions.Logging;

namespace PG.StarWarsGame.Files.DAT.Services
{
    [Export(nameof(IDatFileUtilityService))]
    public class DatFileUtilityService : IDatFileUtilityService
    {
        private const string LANGUAGE_FILE_NAME_ENGLISH = "ENGLISH";
        private const string LANGUAGE_FILE_NAME_GERMAN = "GERMAN";
        private const string LANGUAGE_FILE_NAME_FRENCH = "FRENCH";
        private const string LANGUAGE_FILE_NAME_ITALIAN = "ITALIAN";
        private const string LANGUAGE_FILE_NAME_RUSSIAN = "RUSSIAN";
        private const string LANGUAGE_FILE_NAME_SPANISH = "SPANISH";
        private const string LANGUAGE_FILE_NAME_KOREAN = "KOREAN";
        private const string LANGUAGE_ISO_CODE_ENGLISH = "EN";
        private const string LANGUAGE_ISO_CODE_GERMAN = "DE";
        private const string LANGUAGE_ISO_CODE_FRENCH = "FR";
        private const string LANGUAGE_ISO_CODE_ITALIAN = "IT";
        private const string LANGUAGE_ISO_CODE_RUSSIAN = "RU";
        private const string LANGUAGE_ISO_CODE_SPANISH = "ES";
        private const string LANGUAGE_ISO_CODE_KOREAN = "KO";

        private readonly ILogger<DatFileUtilityService> m_logger;
        
        internal DatFileUtilityService(ILoggerFactory loggerFactory = null)
        {
            m_logger = loggerFactory?.CreateLogger<DatFileUtilityService>();
        }

        public bool TryGetCultureInfoFromFileName(string fileName, out CultureInfo cultureInfo)
        {
            if (fileName.Contains(LANGUAGE_FILE_NAME_ENGLISH, StringComparison.InvariantCultureIgnoreCase))
            {
                cultureInfo = new CultureInfo(LANGUAGE_ISO_CODE_ENGLISH);
                return true;
            }

            if (fileName.Contains(LANGUAGE_FILE_NAME_GERMAN, StringComparison.InvariantCultureIgnoreCase))
            {
                cultureInfo = new CultureInfo(LANGUAGE_ISO_CODE_GERMAN);
                return true;
            }

            if (fileName.Contains(LANGUAGE_FILE_NAME_FRENCH, StringComparison.InvariantCultureIgnoreCase))
            {
                cultureInfo = new CultureInfo(LANGUAGE_ISO_CODE_FRENCH);
                return true;
            }

            if (fileName.Contains(LANGUAGE_FILE_NAME_ITALIAN, StringComparison.InvariantCultureIgnoreCase))
            {
                cultureInfo = new CultureInfo(LANGUAGE_ISO_CODE_ITALIAN);
                return true;
            }

            if (fileName.Contains(LANGUAGE_FILE_NAME_RUSSIAN, StringComparison.InvariantCultureIgnoreCase))
            {
                cultureInfo = new CultureInfo(LANGUAGE_ISO_CODE_RUSSIAN);
                return true;
            }

            if (fileName.Contains(LANGUAGE_FILE_NAME_SPANISH, StringComparison.InvariantCultureIgnoreCase))
            {
                cultureInfo = new CultureInfo(LANGUAGE_ISO_CODE_SPANISH);
                return true;
            }

            if (fileName.Contains(LANGUAGE_FILE_NAME_KOREAN, StringComparison.InvariantCultureIgnoreCase))
            {
                cultureInfo = new CultureInfo(LANGUAGE_ISO_CODE_KOREAN);
                return true;
            }
            m_logger?.LogWarning($"No valid culture mapping found for \"{fileName}\".");
            cultureInfo = null;
            return false;
        }

        public bool TryGetFileNamePartFromCultureInfo(CultureInfo cultureInfo, out string fileNamePart)
        {
            if (cultureInfo.Name.EndsWith(LANGUAGE_ISO_CODE_ENGLISH, StringComparison.InvariantCultureIgnoreCase))
            {
                fileNamePart = LANGUAGE_FILE_NAME_ENGLISH;
                return true;
            }

            if (cultureInfo.Name.EndsWith(LANGUAGE_ISO_CODE_FRENCH, StringComparison.InvariantCultureIgnoreCase))
            {
                fileNamePart = LANGUAGE_FILE_NAME_FRENCH;
                return true;
            }

            if (cultureInfo.Name.EndsWith(LANGUAGE_ISO_CODE_GERMAN, StringComparison.InvariantCultureIgnoreCase))
            {
                fileNamePart = LANGUAGE_FILE_NAME_GERMAN;
                return true;
            }

            if (cultureInfo.Name.EndsWith(LANGUAGE_ISO_CODE_KOREAN, StringComparison.InvariantCultureIgnoreCase))
            {
                fileNamePart = LANGUAGE_FILE_NAME_KOREAN;
                return true;
            }

            if (cultureInfo.Name.EndsWith(LANGUAGE_ISO_CODE_ITALIAN, StringComparison.InvariantCultureIgnoreCase))
            {
                fileNamePart = LANGUAGE_FILE_NAME_ITALIAN;
                return true;
            }

            if (cultureInfo.Name.EndsWith(LANGUAGE_ISO_CODE_RUSSIAN, StringComparison.InvariantCultureIgnoreCase))
            {
                fileNamePart = LANGUAGE_FILE_NAME_RUSSIAN;
                return true;
            }

            if (cultureInfo.Name.EndsWith(LANGUAGE_ISO_CODE_SPANISH, StringComparison.InvariantCultureIgnoreCase))
            {
                fileNamePart = LANGUAGE_FILE_NAME_SPANISH;
                return true;
            }
            m_logger?.LogWarning($"No valid file name part mapping found for \"{cultureInfo}\".");

            fileNamePart = null;
            return false;
        }

        public string ConfiguredDefaultLanguageFileNamePart => LANGUAGE_FILE_NAME_ENGLISH;
        public CultureInfo ConfiguredDefaultCultureInfo => new CultureInfo(LANGUAGE_ISO_CODE_ENGLISH);
    }
}