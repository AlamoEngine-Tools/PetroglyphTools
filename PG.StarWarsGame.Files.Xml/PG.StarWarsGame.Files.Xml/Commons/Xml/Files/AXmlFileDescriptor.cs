using System.Linq;
using System.Reflection;
using PG.Commons.Xml;
using PG.Commons.Xml.Tags;
using PG.StarWarsGame.Files.Xml.Resources;

namespace PG.StarWarsGame.Files.Xml.Commons.Xml.Files;

public abstract class AXmlFileDescriptor : IXmlFileDescriptor
{
    public virtual string Description => LocalizableTexts.NoDescriptionAvailable;

    public bool Contains<T>() where T : IXmlTagDescriptor
    {
        var propInfos = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        return propInfos.Any(propertyInfo => propertyInfo.CanRead && propertyInfo.PropertyType == typeof(T));
    }

    public T GetByClass<T>() where T : IXmlTagDescriptor
    {
        var propInfos = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var propertyInfo in propInfos)
        {
            if (propertyInfo.CanRead && propertyInfo.PropertyType == typeof(T))
            {
                return (T) propertyInfo.GetValue(this);
            }
        }

        throw new PropertyNotFoundException($"A property of type {typeof(T)} could not be found.");
    }
}
