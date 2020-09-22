using CsvHelper.Configuration.Attributes;

namespace PG.StarWarsGame.Localisation.Data.Config.Csv
{
    internal class Localisation
    {
        [Index(0)] internal string Key { get; set; }
        [Index(1)] internal string Text { get; set; }
    }
}