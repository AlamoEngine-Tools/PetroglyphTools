// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using PG.Commons.Util;
using PG.Core.Data.Etl.Transform;
using PG.Core.Localisation;
using PG.Core.Services;
using PG.StarWarsGame.Files.DAT.Files;
using PG.StarWarsGame.Localisation.Data.Etl.Extract;
using PG.StarWarsGame.Localisation.Util;

namespace PG.StarWarsGame.Localisation.Data.Etl.Transform
{
    internal class SortedTranslationTransformService : AService<SortedTranslationTransformService>,
        ITransformService<SortedTranslationStage1Bean, SortedTranslationStage2Bean>
    {
        private readonly List<SortedTranslationStage2Bean> m_stage2Beans;
        private readonly List<SortedTranslationStage2Bean> m_badBeans;
        private readonly SortedDatAlamoFileType m_alamoFileType = new SortedDatAlamoFileType();

        public SortedTranslationTransformService(IFileSystem fileSystem,
            IReadOnlyList<SortedTranslationStage1Bean> stage1Beans, ILoggerFactory loggerFactory = null) : base(fileSystem,
            loggerFactory)
        {
            Stage1Beans = stage1Beans ?? throw new ArgumentNullException(nameof(stage1Beans));
            m_stage2Beans = new List<SortedTranslationStage2Bean>();
            m_badBeans = new List<SortedTranslationStage2Bean>();
        }

        public IReadOnlyList<SortedTranslationStage1Bean> Stage1Beans { get; }

        public IReadOnlyList<SortedTranslationStage2Bean> Stage2Beans => m_stage2Beans.AsReadOnly();
        public IReadOnlyList<SortedTranslationStage2Bean> BadBeans => m_badBeans.AsReadOnly();

        public void Transform()
        {
            foreach (SortedTranslationStage1Bean s1Bean in Stage1Beans)
            {
                if (TryTransformItem(s1Bean, out SortedTranslationStage2Bean s2Bean))
                {
                    m_stage2Beans.Add(s2Bean);
                }
                else
                {
                    m_badBeans.Add(s2Bean);
                }
            }
        }

        public bool TryTransformItem(SortedTranslationStage1Bean s1, out SortedTranslationStage2Bean s2)
        {
            string languageDefinitionException = null;

            if (!LocalisationUtility.TryGuessAlamoLanguageDefinitionByIdentifier(
                LanguageIdentifierFromFileName(s1.OriginFileName),
                out IAlamoLanguageDefinition alamoLanguageDefinition))
            {
                languageDefinitionException = CreateTransformException(
                    $"No matching {nameof(IAlamoLanguageDefinition)} found.",
                    nameof(s1.OriginFileName), nameof(s2.LanguageDefinition));
            }

            string key = null;
            string keyException = null;

            if (StringUtility.HasText(s1.Key))
            {
                if (!m_stage2Beans.Any(p => p.Key.Equals(s1.Key, StringComparison.InvariantCultureIgnoreCase)))
                {
                    key = s1.Key;
                }
                else
                {
                    keyException =
                        CreateTransformException($"Duplicate key [{s1.Key}]", nameof(s1.Key), nameof(s1.Key));
                }
            }
            else
            {
                keyException = CreateTransformException($"Invalid key [{s1.Key}]", nameof(s1.Key), nameof(s1.Key));
            }

            string value = null;
            string valueException = null;

            if (s1.Value != null)
            {
                value = s1.Value;
            }
            else
            {
                valueException =
                    CreateTransformException($"Invalid value [{s1.Value}]", nameof(s1.Value), nameof(s1.Value));
            }

            s2 = new SortedTranslationStage2Bean
            {
                LanguageDefinition = alamoLanguageDefinition,
                LanguageDefinitionException = languageDefinitionException,
                Key = key,
                KeyException = keyException,
                Value = value,
                ValueException = valueException,
            };
            return languageDefinitionException == null && keyException == null && valueException == null;
        }

        public string CreateTransformException(string message, string propertyStage1, string propertyStage2,
            Exception e = null)
        {
            StringBuilder builder = new StringBuilder("An error occured whilst transforming property [");
            builder.Append(propertyStage1);
            builder.Append("] to target property [");
            builder.Append(propertyStage2);
            builder.AppendLine("].");
            if (StringUtility.HasText(message))
            {
                builder.AppendLine($"Message: {message}");
            }

            if (e != null)
            {
                builder.AppendLine(e.ToString());
            }

            return builder.ToString();
        }


        private string LanguageIdentifierFromFileName(string fileName)
        {
            if (!StringUtility.HasText(fileName))
            {
                return string.Empty;
            }

            string languageIdentifierFromFileName = FileSystem.Path.GetFileName(fileName).ToUpper();
            languageIdentifierFromFileName = languageIdentifierFromFileName.Replace(".",
                string.Empty, StringComparison.InvariantCultureIgnoreCase);
            languageIdentifierFromFileName = languageIdentifierFromFileName.Replace(m_alamoFileType.FileExtension,
                string.Empty, StringComparison.InvariantCultureIgnoreCase);
            languageIdentifierFromFileName = languageIdentifierFromFileName.Replace(m_alamoFileType.FileBaseName,
                string.Empty, StringComparison.InvariantCultureIgnoreCase);
            languageIdentifierFromFileName = languageIdentifierFromFileName.Replace(
                m_alamoFileType.FileBaseNameSeparator, string.Empty, StringComparison.InvariantCultureIgnoreCase);
            languageIdentifierFromFileName = languageIdentifierFromFileName.Trim();
            return languageIdentifierFromFileName;
        }
    }
}
