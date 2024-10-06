using System;

namespace PG.Commons.Attributes;

/// <summary>
///     Simple annotation to attach platform info.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct)]
public class PlatformAttribute : Attribute
{
    /// <summary>
    ///     .ctor
    /// </summary>
    /// <param name="platform"></param>
    public PlatformAttribute(Platform platform)
    {
        Platform = platform;
    }

    /// <summary>
    ///     The <see cref="Platform" /> flags.
    /// </summary>
    public Platform Platform { get; }
}