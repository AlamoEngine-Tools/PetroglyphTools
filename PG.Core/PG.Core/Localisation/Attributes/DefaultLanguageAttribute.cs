// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.Core.Localisation.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class DefaultLanguageAttribute : Attribute
    {
        private readonly bool m_isDefault;
        public bool IsDefault => m_isDefault;
        
        public DefaultLanguageAttribute()
        {
            m_isDefault = true;
        }

        public DefaultLanguageAttribute(bool isDefault)
        {
            m_isDefault = isDefault;
        }
    }
}
