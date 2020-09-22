namespace PG.StarWarsGame.Localisation.Data.Config
{
    /// <summary>
    /// Localisation versions.
    /// </summary>
    public enum ConfigVersion
    {
        /// <summary>
        /// Invalid version.
        /// </summary>
        Invalid,
        /// <summary>
        /// XMl text project as exported by the EaW Text Editor
        /// </summary>
        EaWTextEditorXml,
        /// <summary>
        /// Hierarchy-based text project as suggested by Petroglyph Engine Tools.
        /// </summary>
        HierarchicalTextProject,
        /// <summary>
        /// Single file text project as produced by TR's datassembler. 
        /// </summary>
        SingleFileCsv,
        /// <summary>
        /// Multiple dat files - the base game format.
        /// </summary>
        DatFiles
    }
}