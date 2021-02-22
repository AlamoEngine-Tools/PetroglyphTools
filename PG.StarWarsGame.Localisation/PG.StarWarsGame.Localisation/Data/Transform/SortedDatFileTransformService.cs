// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using Microsoft.Extensions.Logging;
using PG.Core.Data.Transform;
using PG.Core.Services;
using PG.StarWarsGame.Localisation.Data.Extract;

namespace PG.StarWarsGame.Localisation.Data.Transform
{
    public class SortedDatFileTransformService : AService<SortedDatFileTransformService>,
        ITransformService<SortedTranslationStage1Bean, SortedTranslationStage2Bean>
    {
        private readonly List<SortedTranslationStage2Bean> m_stage2Beans;

        public SortedDatFileTransformService(IFileSystem fileSystem,
            IReadOnlyList<SortedTranslationStage1Bean> stage1Beans, ILoggerFactory loggerFactory) : base(fileSystem, loggerFactory)
        {
            Stage1Beans = stage1Beans ?? throw new ArgumentNullException(nameof(stage1Beans));
        }

        public IReadOnlyList<SortedTranslationStage1Bean> Stage1Beans { get; }

        public IReadOnlyList<SortedTranslationStage2Bean> Stage2Beans => m_stage2Beans.AsReadOnly();
    }
}
