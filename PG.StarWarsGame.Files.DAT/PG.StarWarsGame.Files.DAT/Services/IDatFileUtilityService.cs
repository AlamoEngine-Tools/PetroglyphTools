using System.Globalization;

namespace PG.StarWarsGame.Files.DAT.Services
{
    /// <summary>
    /// MEF service interface definition for handling various utility operations required for working with *.DAT files.
    /// A default implementation is provided in <see cref="DatFileUtilityService"/>.
    /// When requesting the default implementation via an IoC Container or registering via injection, you may pass
    /// a logger implementing <see cref="Microsoft.Extensions.Logging.ILogger"/>
    /// </summary>
    public interface IDatFileUtilityService
    {
        /// <summary>
        /// Tries to guess the correct <see cref="CultureInfo"/> for a given filename.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        bool TryGetCultureInfoFromFileName(string fileName, out CultureInfo cultureInfo);
        
        /// <summary>
        /// Tries to guess the correct file name builder part from a give <see cref="CultureInfo"/>.
        /// </summary>
        /// <param name="cultureInfo"></param>
        /// <param name="fileNamePart"></param>
        /// <returns></returns>
        bool TryGetFileNamePartFromCultureInfo(CultureInfo cultureInfo, out string fileNamePart);

        /// <summary>
        /// Returns the configured default language file name builder part.
        /// The provided default implementation <see cref="DatFileUtilityService"/> is configured with
        /// "ENGLISH" as default.
        /// </summary>
        string ConfiguredDefaultLanguageFileNamePart { get; }
        /// <summary>
        /// Returns the configured default <see cref="CultureInfo"/>.
        /// The provided default implementation <see cref="DatFileUtilityService"/> is configured with
        /// English ("en-EN") as default. 
        /// </summary>
        CultureInfo ConfiguredDefaultCultureInfo { get; }
    }
}