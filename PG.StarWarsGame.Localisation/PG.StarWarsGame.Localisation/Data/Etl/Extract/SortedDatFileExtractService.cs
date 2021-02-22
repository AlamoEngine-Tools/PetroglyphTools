// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using PG.Core.Data.Etl.Extract;
using PG.Core.Services;
using PG.StarWarsGame.Files.DAT.Services;

namespace PG.StarWarsGame.Localisation.Data.Etl.Extract
{
    public class SortedDatFileExtractService : AService<SortedDatFileExtractService>, IExtractService<SortedTranslationStage1Bean>
    {
        private readonly ISortedDatFileProcessService m_processService;

        private readonly List<SortedTranslationStage1Bean> m_stage1Beans = new List<SortedTranslationStage1Bean>();

        public SortedDatFileExtractService([CanBeNull] IFileSystem fileSystem,
            [NotNull] ISortedDatFileProcessService processService,
            ILoggerFactory loggerFactory = null) : base(fileSystem, loggerFactory)
        {
            m_processService = processService ?? throw new ArgumentNullException(nameof(processService));
        }

        public IList<SortedTranslationStage1Bean> Stage1Beans => m_stage1Beans.AsReadOnly();

        public void Extract()
        {
            throw new NotImplementedException();
        }
    }
}
