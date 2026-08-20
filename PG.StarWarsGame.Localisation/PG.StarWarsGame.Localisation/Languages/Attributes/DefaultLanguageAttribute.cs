// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.StarWarsGame.Localisation.Languages.Attributes
{
    /// <summary>
    /// Marks a language definition as the default game language.
    /// Only one concrete <see cref="IAlamoLanguageDefinition"/> implementation may carry this attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class DefaultLanguageAttribute : Attribute
    {
    }
}
