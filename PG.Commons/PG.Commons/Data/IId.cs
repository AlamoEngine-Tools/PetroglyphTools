using System;

namespace PG.Commons.Data;

/// <summary>
///     A generic ID definition
/// </summary>
public interface IId : IEquatable<IId>
{
    /// <summary>
    ///     The arity of the ID.
    /// </summary>
    int Arity { get; }

    /// <summary>
    ///     The string representation of the ID
    /// </summary>
    /// <returns></returns>
    string Unwrap();
}