// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons;

namespace PG.StarWarsGame.Localisation.Baseline.Test;

public abstract class CommonBaselineTestBase : IDisposable
{
    protected ServiceProvider ServiceProvider { get; }

    protected CommonBaselineTestBase()
    {
        var sc = new ServiceCollection();
        PetroglyphCommons.ContributeServices(sc);
        sc.SupportLocalisationBaseline();
        ServiceProvider = sc.BuildServiceProvider();
    }

    public void Dispose() => ServiceProvider.Dispose();
}
