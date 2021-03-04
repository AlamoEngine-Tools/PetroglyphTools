// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Util;

namespace PG.StarWarsGame.Localisation.Data.Config.v2
{
    public enum OverrideType
    {
        Core,
        Expansion,
        Mod
    }

    public static class OverrideTypeExtension
    {
        public static string ToFileNameContribution(this OverrideType e)
        {
            return e.ToString().ToLower();
        }

        public static bool TryParse(this OverrideType _, string s, out OverrideType t)
        {
            if (!StringUtility.HasText(s))
            {
                throw new ArgumentException("No string to parse.", nameof(s));
            }

            s = s.Trim();
            foreach (OverrideType resourceType in Enum.GetValues(typeof(OverrideType)))
            {
                if (!resourceType.ToFileNameContribution().Equals(s, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                t = resourceType;
                return true;
            }

            t = OverrideType.Core;
            return false;
        }
    }
}
