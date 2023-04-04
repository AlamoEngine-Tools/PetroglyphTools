// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Core.Attributes;
using PG.Core.Data.Etl.Source;
using PG.Core.Data.Etl.Target;
using PG.Core.Reporting;
using PG.Core.Services.Attributes;
using PG.StarWarsGame.Localisation.Data.Translation;

namespace PG.StarWarsGame.Localisation.Services
{
    [Order(OrderAttribute.DEFAULT_ORDER)]
    [DefaultServiceImplementation(typeof(LocalisationEditService))]
    public interface ILocalisationEditService
    {
        bool IsInitialised { get; }

        ISourceDescriptor SourceDescriptor { get; }
        ITargetDescriptor TargetDescriptor { get; }

        IReport LoadAndInitialise();
        IReport SaveAndDiscard();

        bool TryGet(TranslationKey key, out TranslationBean bean);
        bool TryAdd(TranslationBean bean);
        bool TryUpdate(TranslationBean bean);
        bool TryRemove(TranslationKey key);
    }
}
