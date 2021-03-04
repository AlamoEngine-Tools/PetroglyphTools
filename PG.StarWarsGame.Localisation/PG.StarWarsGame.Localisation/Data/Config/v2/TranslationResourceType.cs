// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using PG.Commons.Util;

namespace PG.StarWarsGame.Localisation.Data.Config.v2
{
    public enum TranslationResourceType
    {
        Txt,
        Csv,
        Nls
    }

    public static class TranslationResourceTypeExtension
    {
        public static string ToFileExtension(this TranslationResourceType e)
        {
            return e.ToString().ToLower();
        }

        public static bool TryParse(this TranslationResourceType _, string s, out TranslationResourceType t)
        {
            if (!StringUtility.HasText(s))
            {
                throw new ArgumentException("No string to parse.", nameof(s));
            }

            s = s.Trim();
            foreach (TranslationResourceType resourceType in Enum.GetValues(typeof(TranslationResourceType)))
            {
                if (!resourceType.ToFileExtension().Equals(s, StringComparison.InvariantCultureIgnoreCase))
                {
                    continue;
                }

                t = resourceType;
                return true;
            }

            t = TranslationResourceType.Txt;
            return false;
        }
    }
}
