// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Core.Attributes;
using PG.Core.Localisation;

namespace PG.StarWarsGame.Localisation.Services
{
    [Order(OrderAttribute.DEFAULT_ORDER)]
    public interface ILocalisationProviderService<T> where T : IAlamoLanguageDefinition
    {
        bool TryGet(string key, out string value);
    }
}
