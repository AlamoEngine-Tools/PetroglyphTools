using PG.Commons.Xml.Tags;

namespace PG.Commons.Xml;

/// <summary>
/// A generic description of an XML file structure used by Petroglyph to describe game data.
/// </summary>
public interface IXmlFileDescriptor
{
    /// <summary>
    /// A localised description of the file.
    /// </summary>
    string Description { get; }
    /// <summary>
    /// Checks whether a given <see cref="IXmlFileDescriptor"/> contains a certain <see cref="IXmlTagDescriptor"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    bool Contains<T>() where T : IXmlTagDescriptor;
    /// <summary>
    /// Returns a given <see cref="IXmlTagDescriptor"/> if contained.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T GetByClass<T>() where T : IXmlTagDescriptor;
}