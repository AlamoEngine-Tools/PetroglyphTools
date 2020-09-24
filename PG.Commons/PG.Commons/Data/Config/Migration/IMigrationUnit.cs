namespace PG.Commons.Data.Config.Migration
{
    /// <summary>
    /// Base interface shared by all configuration migration units.
    /// </summary>
    public interface IMigrationUnit
    {
        /// <summary>
        /// Executes a migration.
        /// </summary>
        /// <returns></returns>
        void Migrate();
    }
}