// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Diagnostics.CodeAnalysis;

namespace PG.Core.Localisation.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    [ExcludeFromCodeCoverage]
    public sealed class OfficiallySupportedLanguageAttribute : Attribute
    {
        private readonly bool m_isOfficiallySupported;
        public bool IsOfficiallySupported => m_isOfficiallySupported;

        public OfficiallySupportedLanguageAttribute()
        {
            m_isOfficiallySupported = true;
        }

        public OfficiallySupportedLanguageAttribute(bool isOfficiallySupported)
        {
            m_isOfficiallySupported = isOfficiallySupported;
        }
    }
}
