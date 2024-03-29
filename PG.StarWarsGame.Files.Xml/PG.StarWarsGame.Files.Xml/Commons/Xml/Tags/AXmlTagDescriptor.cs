using PG.Commons.Xml.Tags;
using PG.Commons.Xml.Values;
using PG.StarWarsGame.Files.Xml.Resources;

namespace PG.StarWarsGame.Files.Xml.Commons.Xml.Tags;

/// <summary>
///     Abstract base implementation of the <see cref="IXmlTagDescriptor" /> interface.
/// </summary>
public abstract class AXmlTagDescriptor : IXmlTagDescriptor
{
    public virtual string Description => LocalizableTexts.NoDescriptionAvailable;
    public abstract string Id { get; }
    public abstract XmlValueType Type { get; }
    public abstract XmlValueTypeInternal TypeInternal { get; }
    public abstract bool IsSingleton { get; }
}
