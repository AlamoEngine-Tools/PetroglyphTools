// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.StarWarsGame.Localisation.Languages.Attributes
{
    /// <summary>
    /// Marks a language definition as officially supported by the Alamo engine.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class OfficiallySupportedLanguageAttribute : Attribute
    {
    }
}
