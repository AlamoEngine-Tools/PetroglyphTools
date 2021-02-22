// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Util;

namespace PG.StarWarsGame.Localisation.Data.Config.v2
{
    public enum GameType
    {
        EaW,
        FoC,
        Mod
    }

    public static class GameTypeTypeExtension
    {
        public static string ToFileNameContribution(this GameType e)
        {
            return e.ToString().ToLower();
        }

        public static bool TryParse(this GameType _, string s, out GameType t)
        {
            if (!StringUtility.HasText(s))
            {
                throw new ArgumentException("No string to parse.", nameof(s));
            }

            s = s.Trim();
            foreach (GameType resourceType in Enum.GetValues(typeof(GameType)))
            {
                if (!resourceType.ToFileNameContribution().Equals(s, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                t = resourceType;
                return true;
            }

            t = GameType.EaW;
            return false;
        }
    }
}
