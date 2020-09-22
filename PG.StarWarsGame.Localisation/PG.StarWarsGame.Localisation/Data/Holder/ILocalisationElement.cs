using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;

namespace PG.StarWarsGame.Localisation.Data.Holder
{
    internal interface ILocalisationElement
    {
        bool TryAdd([NotNull] CultureInfo key, string value);
        bool TryUpdate([NotNull] CultureInfo key, string value);
        string Get([NotNull] CultureInfo key);
        IEnumerable<string> GetAll();
        IEnumerable<CultureInfo> GetAllKeys();
    }
}