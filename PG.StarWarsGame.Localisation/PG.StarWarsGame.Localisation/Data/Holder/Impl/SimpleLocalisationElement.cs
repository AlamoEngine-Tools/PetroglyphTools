using System.Collections.Generic;
using System.Globalization;

namespace PG.StarWarsGame.Localisation.Data.Holder.Impl
{
    internal class SimpleLocalisationElement : ALocalisationElement
    {
        internal SimpleLocalisationElement(Dictionary<CultureInfo, string> localisation) : base(localisation)
        {
        }

        internal static SimpleLocalisationElement GetEmptyElement()
        {
            return new SimpleLocalisationElement(new Dictionary<CultureInfo, string>());
        }
    }
}