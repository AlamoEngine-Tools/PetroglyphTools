using System;

namespace PG.Commons.Attributes;

/// <summary>
///     The platform a feature/... is supported on.
/// </summary>
[Flags]
public enum Platform
{
    /// <summary>
    ///     Disc version of the game / expansion
    /// </summary>
    Disc = 0b00001,

    /// <summary>
    ///     Steam version of the game / expansion
    /// </summary>
    Steam = 0b00010,

    /// <summary>
    ///     Origin version of the game / expansion - yep that abomination exists.
    /// </summary>
    Origin = 0b00100,

    /// <summary>
    ///     GoG.com version of the game.
    /// </summary>
    GoG = 0b01000,

    /// <summary>
    ///     MAC version of the game / expansion - yep that exists as well, only the base game though.
    /// </summary>
    Mac = 0b10000,

    /// <summary>
    ///     Versions that do no longer receive updates.
    /// </summary>
    Outdated = Disc | Origin | Steam | GoG | Mac
}