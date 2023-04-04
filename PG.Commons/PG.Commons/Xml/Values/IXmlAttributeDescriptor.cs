// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Xml.Tags;

namespace PG.Commons.Xml.Values
{
    public interface IXmlAttributeDescriptor
    {
        IXmlTagDescriptor XmlTagDescriptor { get; }
    }
}
