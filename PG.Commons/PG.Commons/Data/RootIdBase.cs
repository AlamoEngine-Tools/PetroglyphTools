using System;

namespace PG.Commons.Data;

/// <inheritdoc cref="PG.Commons.Data.IdBase" />
public abstract class RootIdBase<T> : IdBase, IComparable<RootIdBase<T>> where T : class, IComparable<T>
{
    /// <inheritdoc />
    protected RootIdBase(T rawId) : base(rawId)
    {
    }

    /// <inheritdoc />
    public int CompareTo(RootIdBase<T>? other)
    {
        return other == null ? 1 : Raw().CompareTo(other.Raw());
    }

    /// <inheritdoc />
    protected override int GetConfiguredArity()
    {
        return 1;
    }

    /// <inheritdoc />
    protected override bool IsNullId()
    {
        return Components[0] == null;
    }

    /// <summary>
    ///     Raw value.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public T Raw()
    {
        return Components[0] as T ?? throw new InvalidOperationException();
    }
}