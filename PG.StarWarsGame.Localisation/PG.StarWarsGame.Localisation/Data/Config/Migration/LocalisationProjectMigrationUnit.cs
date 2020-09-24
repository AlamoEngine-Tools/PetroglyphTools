using System;
using System.IO.Abstractions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using PG.Commons.Data.Config.Migration;
using PG.Commons.Exceptions;

namespace PG.StarWarsGame.Localisation.Data.Config.Migration
{
    internal class LocalisationProjectMigrationUnit : IMigrationUnit
    {
        [CanBeNull] private readonly ILogger m_logger;
        [NotNull] private readonly IFileSystem m_fileSystem;
        private readonly ConfigVersion m_migrateFrom;
        private readonly ConfigVersion m_migrateTo;
        [NotNull] private readonly string m_pathFrom;
        [NotNull] private readonly string m_pathTo;

        internal LocalisationProjectMigrationUnit(ConfigVersion migrateFrom, [NotNull] string pathFrom, ConfigVersion migrateTo, [NotNull] string pathTo, IFileSystem fileSystem, ILoggerFactory loggerFactory = null)
        {
            m_logger = loggerFactory?.CreateLogger<LocalisationProjectMigrationUnit>();
            m_fileSystem = fileSystem ?? new FileSystem();
            m_migrateFrom = migrateFrom;
            m_migrateTo = migrateTo;
            m_pathFrom = pathFrom ?? throw new ArgumentNullException(nameof(pathFrom));
            m_pathTo = pathTo ?? throw new ArgumentNullException(nameof(pathTo));
        }

        public void Migrate()
        {
            switch (m_migrateFrom)
            {
                case ConfigVersion.Invalid:
                    throw new InvalidVersionException(nameof(ConfigVersion.Invalid));
                case ConfigVersion.EaWTextEditorXml:
                    MigrateEaWTextEditorXml();
                    break;
                case ConfigVersion.HierarchicalTextProject:
                    MigrateHierarchicalTextProject();
                    break;
                case ConfigVersion.SingleFileCsv:
                    MigrateSingleFileCsv();
                    break;
                case ConfigVersion.DatFiles:
                    MigrateDatFiles();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(m_migrateFrom), m_migrateFrom, string.Empty);
            }
        }

        private void MigrateDatFiles()
        {
            switch (m_migrateTo)
            {
                case ConfigVersion.Invalid:
                    throw new InvalidVersionException(nameof(ConfigVersion.Invalid));
                case ConfigVersion.EaWTextEditorXml:
                    MigrateDatFilesToEaWTextEditorXml();
                    break;
                case ConfigVersion.HierarchicalTextProject:
                    MigrateDatFilesToHierarchicalTextProject();
                    break;
                case ConfigVersion.SingleFileCsv:
                    MigrateDatFilesToSingleFileCsv();
                    break;
                case ConfigVersion.DatFiles:
                    m_logger?.LogInformation(
                        $"The requested conversion from {nameof(ConfigVersion.DatFiles)} to {nameof(ConfigVersion.DatFiles)} is unnecessary.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(m_migrateTo), m_migrateTo, string.Empty);
            }
        }

        private void MigrateSingleFileCsv()
        {
            switch (m_migrateTo)
            {
                case ConfigVersion.Invalid:
                    throw new InvalidVersionException(nameof(ConfigVersion.Invalid));
                case ConfigVersion.EaWTextEditorXml:
                    MigrateSingleFileCsvToEaWTextEditorXml();
                    break;
                case ConfigVersion.HierarchicalTextProject:
                    MigrateSingleFileCsvToHierarchicalTextProject();
                    break;
                case ConfigVersion.SingleFileCsv:
                    m_logger?.LogInformation(
                        $"The requested conversion from {nameof(ConfigVersion.SingleFileCsv)} to {nameof(ConfigVersion.SingleFileCsv)} is unnecessary.");
                    break;
                case ConfigVersion.DatFiles:
                    MigrateSingleFileCsvToDatFiles();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(m_migrateTo), m_migrateTo, string.Empty);
            }
        }

        private void MigrateHierarchicalTextProject()
        {
            switch (m_migrateTo)
            {
                case ConfigVersion.Invalid:
                    throw new InvalidVersionException(nameof(ConfigVersion.Invalid));
                case ConfigVersion.EaWTextEditorXml:
                    MigrateHierarchicalTextProjectToEaWTextEditorXml();
                    break;
                case ConfigVersion.HierarchicalTextProject:
                    m_logger?.LogInformation(
                        $"The requested conversion from {nameof(ConfigVersion.HierarchicalTextProject)} to {nameof(ConfigVersion.HierarchicalTextProject)} is unnecessary.");
                    break;
                case ConfigVersion.SingleFileCsv:
                    MigrateHierarchicalTextProjectToSingleFileCsv();
                    break;
                case ConfigVersion.DatFiles:
                    MigrateHierarchicalTextProjectToDatFiles();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(m_migrateTo), m_migrateTo, string.Empty);
            }
        }

        private void MigrateEaWTextEditorXml()
        {
            switch (m_migrateTo)
            {
                case ConfigVersion.Invalid:
                    throw new InvalidVersionException(nameof(ConfigVersion.Invalid));
                case ConfigVersion.EaWTextEditorXml:
                    m_logger?.LogInformation(
                        $"The requested conversion from {nameof(ConfigVersion.EaWTextEditorXml)} to {nameof(ConfigVersion.EaWTextEditorXml)} is unnecessary.");
                    break;
                case ConfigVersion.HierarchicalTextProject:
                    MigrateEaWTextEditorXmlToHierarchicalTextProject();
                    break;
                case ConfigVersion.SingleFileCsv:
                    MigrateEaWTextEditorXmlToSingleFileCsv();
                    break;
                case ConfigVersion.DatFiles:
                    MigrateEaWTextEditorXmlToDatFiles();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(m_migrateTo), m_migrateTo, string.Empty);
            }
        }

        private void MigrateDatFilesToSingleFileCsv()
        {
            throw new NotImplementedException();
        }

        private void MigrateDatFilesToHierarchicalTextProject()
        {
            throw new NotImplementedException();
        }

        private void MigrateDatFilesToEaWTextEditorXml()
        {
            throw new NotImplementedException();
        }

        private void MigrateSingleFileCsvToDatFiles()
        {
            throw new NotImplementedException();
        }

        private void MigrateSingleFileCsvToHierarchicalTextProject()
        {
            throw new NotImplementedException();
        }

        private void MigrateSingleFileCsvToEaWTextEditorXml()
        {
            throw new NotImplementedException();
        }

        private void MigrateHierarchicalTextProjectToDatFiles()
        {
            throw new NotImplementedException();
        }

        private void MigrateHierarchicalTextProjectToSingleFileCsv()
        {
            throw new NotImplementedException();
        }

        private void MigrateHierarchicalTextProjectToEaWTextEditorXml()
        {
            throw new NotImplementedException();
        }

        private void MigrateEaWTextEditorXmlToDatFiles()
        {
            throw new NotImplementedException();
        }

        private void MigrateEaWTextEditorXmlToSingleFileCsv()
        {
            throw new NotImplementedException();
        }

        private void MigrateEaWTextEditorXmlToHierarchicalTextProject()
        {
            throw new NotImplementedException();
        }
    }
}