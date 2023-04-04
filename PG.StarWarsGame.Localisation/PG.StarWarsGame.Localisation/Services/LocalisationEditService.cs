// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.IO.Abstractions;
using Microsoft.Extensions.Logging;
using PG.Core.Data.Etl.Source;
using PG.Core.Data.Etl.Target;
using PG.Core.Reporting;
using PG.Core.Services;
using PG.StarWarsGame.Localisation.Data.Translation;

namespace PG.StarWarsGame.Localisation.Services
{
    public class LocalisationEditService : AbstractService<LocalisationEditService>, ILocalisationEditService
    {
        private bool m_isInitialised = false;

        public LocalisationEditService(ISourceDescriptor sourceDescriptor, ITargetDescriptor targetDescriptor, IServiceProvider services) : base(services)
        {
            SourceDescriptor = sourceDescriptor;
            TargetDescriptor = targetDescriptor;
        }

        public bool IsInitialised => m_isInitialised;

        public ISourceDescriptor SourceDescriptor { get; }
        public ITargetDescriptor TargetDescriptor { get; }

        public IReport LoadAndInitialise()
        {
            throw new System.NotImplementedException();
        }

        public IReport SaveAndDiscard()
        {
            throw new System.NotImplementedException();
        }

        public bool TryGet(TranslationKey key, out TranslationBean bean)
        {
            throw new System.NotImplementedException();
        }

        public bool TryAdd(TranslationBean bean)
        {
            throw new System.NotImplementedException();
        }

        public bool TryUpdate(TranslationBean bean)
        {
            throw new System.NotImplementedException();
        }

        public bool TryRemove(TranslationKey key)
        {
            throw new System.NotImplementedException();
        }
    }
}
