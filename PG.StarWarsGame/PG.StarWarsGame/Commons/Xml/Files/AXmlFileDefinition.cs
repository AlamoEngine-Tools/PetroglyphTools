using System.Linq;
using System.Reflection;
using PG.Commons.Exceptions;
using PG.Commons.Xml;
using PG.Commons.Xml.Tags;

namespace PG.StarWarsGame.Commons.Xml.Files
{
    public abstract class AXmlFileDefinition : IXmlFileDefinition
    {
        public bool ContainsProperty<T>() where T : IXmlTagDefinition
        {
            PropertyInfo[] propInfos = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            return propInfos.Any(propertyInfo => propertyInfo.CanRead && propertyInfo.PropertyType == typeof(T));
        }

        public T GetPropertyByClass<T>() where T : IXmlTagDefinition
        {
            PropertyInfo[] propInfos = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo propertyInfo in propInfos)
            {
                if (propertyInfo.CanRead && propertyInfo.PropertyType == typeof(T))
                {
                    return (T) propertyInfo.GetValue(this);
                }
            }

            throw new PropertyNotFoundException($"A property of type {typeof(T)} could not be found.");
        }
    }
}
