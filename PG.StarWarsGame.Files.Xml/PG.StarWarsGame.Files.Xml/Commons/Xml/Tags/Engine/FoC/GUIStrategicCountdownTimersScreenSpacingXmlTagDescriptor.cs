// Copyright (c) Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using PG.Commons.Xml.Values;

namespace PG.StarWarsGame.Files.Xml.Commons.Xml.Tags.Engine.FoC;

public sealed class GUIStrategicCountdownTimersScreenSpacingXmlTagDescriptor : AXmlTagDescriptor
{
    private const string KEY_ID = "GUI_Strategic_Countdown_Timers_Screen_Spacing";
    private const XmlValueType PG_TYPE = XmlValueType.Float;
    private const XmlValueTypeInternal INTERNAL_TYPE = XmlValueTypeInternal.Undefined;
    private const bool IS_SINGLETON = true;

    public override string Id => KEY_ID;
    public override XmlValueType Type => PG_TYPE;
    public override XmlValueTypeInternal TypeInternal => INTERNAL_TYPE;
    public override bool IsSingleton => IS_SINGLETON;
}