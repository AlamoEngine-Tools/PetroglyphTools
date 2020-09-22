using System.Collections.Generic;
using System.Globalization;

namespace PG.StarWarsGame.Localisation.Data.Holder
{
    internal class LocalisationElement : ALocalisationElement
    {
        internal LocalisationElement(Dictionary<CultureInfo, string> localisation) : base(localisation)
        {
        }

        internal static LocalisationElement GetEmptyElement()
        {
            return new LocalisationElement(new Dictionary<CultureInfo, string>());
        }
    }
}