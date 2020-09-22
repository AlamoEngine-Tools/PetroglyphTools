using System.Collections.Generic;
using System.Globalization;
using PG.StarWarsGame.Localisation.Data.Config.HierarchicalTextProject;

namespace PG.StarWarsGame.Localisation.Data.Holder
{
    internal class HierarchicalLocalisationElement : ALocalisationElement
    {

        internal OverrideType OverrideType { get; set; }

        internal HierarchicalLocalisationElement(Dictionary<CultureInfo, string> localisation) : base(localisation)
        {
            OverrideType = OverrideType.Core;
        }
        internal HierarchicalLocalisationElement(Dictionary<CultureInfo, string> localisation, OverrideType overrideType) : base(localisation)
        {
            OverrideType = overrideType;
        }

        internal static HierarchicalLocalisationElement GetEmptyElement()
        {
            return new HierarchicalLocalisationElement(new Dictionary<CultureInfo, string>());
        }
    }
}