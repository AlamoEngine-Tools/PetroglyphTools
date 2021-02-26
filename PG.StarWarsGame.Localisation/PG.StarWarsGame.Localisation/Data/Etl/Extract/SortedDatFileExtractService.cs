// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using PG.Commons.Util;
using PG.Core.Data.Etl.Extract;
using PG.Core.Services;
using PG.StarWarsGame.Files.DAT.Holder;
using PG.StarWarsGame.Files.DAT.Services;

namespace PG.StarWarsGame.Localisation.Data.Etl.Extract
{
    internal class SortedDatFileExtractService : AService<SortedDatFileExtractService>,
        IExtractService<SortedTranslationStage1Bean>
    {
        private readonly ISortedDatFileProcessService m_processService;
        private readonly string m_stage0FilePath;

        private readonly List<SortedTranslationStage1Bean> m_stage1Beans = new List<SortedTranslationStage1Bean>();

        public SortedDatFileExtractService([CanBeNull] IFileSystem fileSystem,
            [NotNull] ISortedDatFileProcessService processService, [NotNull] string stage0FilePath,
            ILoggerFactory loggerFactory = null) : base(fileSystem, loggerFactory)
        {
            m_processService = processService ?? throw new ArgumentNullException(nameof(processService));
            if (!StringUtility.HasText(stage0FilePath))
            {
                throw new ArgumentException("Required argument is null or empty.", nameof(stage0FilePath));
            }
            m_stage0FilePath = stage0FilePath;
        }

        public IList<SortedTranslationStage1Bean> Stage1Beans => m_stage1Beans.AsReadOnly();

        public void Extract()
        {
            SortedDatFileHolder sortedDatFileHolder = m_processService.LoadFromFile(m_stage0FilePath);
            Dictionary<string, string> dict = sortedDatFileHolder.ToDictionary();
            string id = Path.GetFileName(m_stage0FilePath);
            foreach ((string key, string value) in dict)
            {
                m_stage1Beans.Add(new SortedTranslationStage1Bean
                {
                    Key = key,
                    Value = value,
                    OriginFileName = id
                });
            }
        }
    }
}
