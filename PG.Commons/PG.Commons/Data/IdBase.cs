using System;
using System.Linq;
using System.Text;
using PG.Commons.Exceptions;

namespace PG.Commons.Data;

/// <inheritdoc />
public abstract class IdBase : IId
{
    /// <summary>
    ///     The ID components.
    /// </summary>
    protected readonly object?[] Components;

    /// <summary>
    ///     .ctor
    /// </summary>
    protected IdBase(params object[] components)
    {
        if (components.Length != Arity) throw new ArgumentException("Invalid number of components");
        Components = new object[Arity];
        for (var i = 0; i < Components.Length; i++) Components[i] = components[i];
    }

    /// <inheritdoc />
    public bool Equals(IId? other)
    {
        return other != null && Arity == other.Arity && GetHashCode().Equals(other.GetHashCode());
    }

    /// <inheritdoc />
    public int Arity => GetConfiguredArity();

    /// <inheritdoc />
    public string Unwrap()
    {
        var b = new StringBuilder();
        b.Append(GetType().Name).Append('[');
        foreach (var idComponent in Components) b.Append(idComponent).Append(';');
        b.Remove(b.Length - 1, 1); // remove the last ";".
        b.Append(']');
        return b.ToString();
    }

    /// <summary>
    ///     Convenience method to access components in a type-safe manner.
    /// </summary>
    /// <param name="idx"></param>
    /// <param name="type"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentException">If the provided index is out of bounds.</exception>
    /// <exception cref="ArgumentNullException">If no <see cref="Type" /> is provided.</exception>
    /// <exception cref="Exception"></exception>
    protected T? GetIdComponent<T>(int idx, Type type) where T : class
    {
        if (idx > 0 || idx >= Arity) throw new ArgumentException("Invalid component index");
        if (type == null) throw new ArgumentNullException(nameof(type));
        var c = Components[idx];
        if (c?.GetType() != type) throw new TypeMismatchException($"Component {idx - 1} is not of type {type}");
        return c as T;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(GetConfiguredArity(), HashCode.Combine(Components));
    }

    /// <summary>
    ///     Returns true if this ID is equivalent to null.
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsNullId()
    {
        return Components.Any();
    }

    /// <summary>
    ///     The arity of this ID type.
    /// </summary>
    /// <returns></returns>
    protected abstract int GetConfiguredArity();
}