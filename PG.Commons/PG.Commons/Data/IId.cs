using System;

namespace PG.Commons.Data;

/// <summary>
///     A generic ID definition
/// </summary>
public interface IId : IEquatable<IId>, IComparable<IId>, IComparable
{
    /// <summary>
    ///     The arity of the ID.
    /// </summary>
    int Arity { get; }
}
