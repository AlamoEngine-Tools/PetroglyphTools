using PG.StarWarsGame.Localisation.Data.Config;

namespace PG.StarWarsGame.Localisation.Services
{
    public interface ILocalisationProjectMigrationService
    {
        void MigrateLocalisationProject(string localisationProjectPath, ConfigVersion from, ConfigVersion to);
    }
}