// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Linq;
using System.Reflection;
using PG.Commons.Exceptions;
using PG.Commons.Xml;
using PG.Commons.Xml.Tags;
using PG.StarWarsGame.Resources;

namespace PG.StarWarsGame.Files.Xml.Commons.Xml.Files
{
    public abstract class AXmlFileDescriptor : IXmlFileDescriptor
    {
        public virtual string Description => LocalizableTexts.NoDescriptionAvailable;

        public bool Contains<T>() where T : IXmlTagDescriptor
        {
            PropertyInfo[] propInfos = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            return propInfos.Any(propertyInfo => propertyInfo.CanRead && propertyInfo.PropertyType == typeof(T));
        }

        public T GetByClass<T>() where T : IXmlTagDescriptor
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
