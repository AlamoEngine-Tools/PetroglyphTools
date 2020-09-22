using System.Collections.Generic;
using System.Globalization;
using PG.StarWarsGame.Localisation.Data.Config;

namespace PG.StarWarsGame.Localisation.Services
{
    public interface ILocalisationService
    {
        void LoadLocalisationProject(string textProjectDirectoryPath, ConfigVersion textProjectConfigVersion);
        string GetLocalisation(string textKey, CultureInfo cultureInfo);
        IEnumerable<string> GetAllLocalisations(string textKey);
        bool TryUpdateLocalisation(string textKey, string localisation, CultureInfo cultureInfo);
        bool TryAddLocalisation(string textKey, string localisation, CultureInfo cultureInfo);
        void SaveToDisc(string textProjectDirectoryPath, ConfigVersion textProjectConfigVersion, bool cleanDirectory = true);
    }
}