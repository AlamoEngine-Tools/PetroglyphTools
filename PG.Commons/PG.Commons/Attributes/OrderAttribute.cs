using System;

namespace PG.Commons.Attributes;

/// <summary>
///     Attach a sort order to a given class or interface.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class OrderAttribute : Attribute
{
    /// <summary>
    ///     The default order.
    /// </summary>
    public const double DefaultOrder = 2000;

    /// <summary>
    ///     .ctor
    /// </summary>
    /// <param name="order"></param>
    public OrderAttribute(double order)
    {
        Order = order;
    }

    /// <summary>
    ///     The attribute value.
    /// </summary>
    public double Order { get; }
}