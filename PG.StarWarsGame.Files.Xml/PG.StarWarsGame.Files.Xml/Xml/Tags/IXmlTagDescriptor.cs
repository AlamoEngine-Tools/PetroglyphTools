using PG.Commons.Xml.Values;

namespace PG.Commons.Xml.Tags;

/// <summary>
/// Encapsulates all required meta-information about an XML Tag in order to parse and/or validate it.
/// </summary>
public interface IXmlTagDescriptor
{
    /// <summary>
    /// A localised description of the tag.
    /// </summary>
    string Description { get; }
    /// <summary>
    /// The tag name as used in the XML file. 
    /// </summary>
    string Id { get; }
    /// <summary>
    /// The official XML value type as defined in the Data Miner map.
    /// </summary>
    XmlValueType Type { get; }
    /// <summary>
    /// The internal XML type to further refine parsing options.
    /// </summary>
    XmlValueTypeInternal TypeInternal { get; }
    /// <summary>
    /// Defines whether a tag can appear multiple times within an object.
    /// </summary>
    bool IsSingleton { get; }
}