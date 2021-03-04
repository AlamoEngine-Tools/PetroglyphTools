// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using PG.Commons.Util;
using PG.Core.Data.Etl.Load;
using PG.Core.Services;
using PG.StarWarsGame.Localisation.Data.Etl.Stage.Bean;
using PG.StarWarsGame.Localisation.Data.Translation;

namespace PG.StarWarsGame.Localisation.Data.Etl.Load
{
    internal sealed class SortedTranslationLoadService : AService<SortedTranslationLoadService>,
        ILoadService<SortedTranslationStage2Bean, TranslationItem>
    {
        private readonly List<TranslationItem> m_targetBeans;

        public SortedTranslationLoadService([CanBeNull] IFileSystem fileSystem,
            IReadOnlyList<SortedTranslationStage2Bean> stage2Beans, [CanBeNull] ILoggerFactory loggerFactory = null) : base(
            fileSystem, loggerFactory)
        {
            Stage2Beans = stage2Beans ?? throw new ArgumentNullException(nameof(stage2Beans));
            m_targetBeans = new List<TranslationItem>();
        }

        public IReadOnlyList<SortedTranslationStage2Bean> Stage2Beans { get; }
        public IReadOnlyList<TranslationItem> TargetBeans => m_targetBeans.AsReadOnly();

        public void Load()
        {
            foreach (SortedTranslationStage2Bean stage2Bean in Stage2Beans)
            {
                if (TryLoadItem(stage2Bean, out TranslationItem targetBean))
                {
                    m_targetBeans.Add(targetBean);
                }
                else
                {
                    throw new EtlLoadException(
                        $"{nameof(SortedTranslationStage2Bean)} could not be loaded although it transformed successfully.");
                }
            }
        }

        public bool TryLoadItem(SortedTranslationStage2Bean stage2Bean, out TranslationItem targetBean)
        {
            if (StringUtility.HasText(stage2Bean.KeyException) || StringUtility.HasText(stage2Bean.ValueException) ||
                StringUtility.HasText(stage2Bean.LanguageDefinitionException))
            {
                targetBean = null;
                return false;
            }

            targetBean = new TranslationItem(stage2Bean.Key, stage2Bean.LanguageDefinition, stage2Bean.Value);
            return true;
        }
    }
}
