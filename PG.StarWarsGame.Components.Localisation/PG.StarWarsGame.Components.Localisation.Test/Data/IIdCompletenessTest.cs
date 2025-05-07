// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Test.Data;

namespace PG.StarWarsGame.Components.Localisation.Test.Data;

// ReSharper disable once InconsistentNaming
public class IIdCompletenessTest : IIdTestCompletenessTestBase
{
    protected override string GetConfiguredNamespaceBase()
    {
        return "PG.StarWarsGame.Components.Localisation";
    }
}
