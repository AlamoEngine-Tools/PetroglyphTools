using System;

namespace PG.Commons.Exceptions;

/// <summary>
///     Thrown if the provided type does not match the requested type. Usually thrown when a generic request for a key
///     component is not logically sound.
/// </summary>
public class TypeMismatchException : Exception
{
    /// <inheritdoc />
    public TypeMismatchException()
    {
    }

    /// <inheritdoc />
    public TypeMismatchException(string message) : base(message)
    {
    }

    /// <inheritdoc />
    public TypeMismatchException(string message, Exception inner) : base(message, inner)
    {
    }
}