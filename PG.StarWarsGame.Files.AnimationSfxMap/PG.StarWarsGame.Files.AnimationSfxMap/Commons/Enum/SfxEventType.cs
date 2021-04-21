// Copyright (c) 2021 Alamo Engine Tools and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;

namespace PG.StarWarsGame.Files.AnimationSfxMap.Commons.Enum
{
    public enum SfxEventType
    {
        Invalid = 0,
        Sound,
        Surface
    }

    public static class SfxEventTypeExtensions
    {
        public static string AsPetroglyphStringConstant(this SfxEventType sfxEventType)
        {
            switch (sfxEventType)
            {
                case SfxEventType.Sound:
                    return "SOUND";
                case SfxEventType.Surface:
                    return "SURFACE";
                default:
                    throw new ArgumentOutOfRangeException(nameof(sfxEventType), sfxEventType, null);
            }
        }
    }
}
