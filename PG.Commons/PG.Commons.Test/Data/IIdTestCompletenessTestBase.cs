// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PG.Commons.Data;
using Xunit;

namespace PG.Commons.Test.Data;

// ReSharper disable once InconsistentNaming
public abstract class IIdTestCompletenessTestBase
{
    [Fact]
    public void Test_IIdTestPresetForAllIIdsOfPackage()
    {
        var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).ToList();
        var iids = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assemblyTypes => assemblyTypes.GetTypes())
            .Where(assemblyType => typeof(IId).IsAssignableFrom(assemblyType)
                                   && assemblyType is { IsClass: true, IsAbstract: false }
                                   && assemblyType.Namespace != null
                                   && assemblyType.Namespace.StartsWith(GetConfiguredNamespaceBase()))
            .ToList();
        var present = new Dictionary<Type, bool>();


        foreach (var iid in iids)
        {
            present.Add(iid, false);
            if (types.Any(type => iid.Name + "Test" == type.Name)) present[iid] = true;
        }

        var o = new StringBuilder().Append("The following IIds have no corresponding test:\n");
        var missing = false;
        foreach (var kvp in present.Where(kvp => !kvp.Value))
        {
            o.Append($"\t{kvp.Key.FullName}\n");
            missing = true;
        }

        Assert.False(missing, o.ToString());
    }

    protected abstract string GetConfiguredNamespaceBase();
}
