// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Xml.Values;

namespace PG.StarWarsGame.Files.Xml.Commons.Xml.Tags.Engine.FoC;

public sealed class ActivatedSystemSpyAbilityNamesXmlTagDescriptor : AXmlTagDescriptor
{
    private const string KEY_ID = "Activated_System_Spy_Ability_Names";
    private const XmlValueType PG_TYPE = XmlValueType.TypeReferenceList;
    private const XmlValueTypeInternal INTERNAL_TYPE = XmlValueTypeInternal.Undefined;
    private const bool IS_SINGLETON = true;

    public override string Id => KEY_ID;
    public override XmlValueType Type => PG_TYPE;
    public override XmlValueTypeInternal TypeInternal => INTERNAL_TYPE;
    public override bool IsSingleton => IS_SINGLETON;
}