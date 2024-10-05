using System;

namespace PG.Commons.Data;

/// <summary>
///     Container class allowing to map from a data objct to a peer and vice versa.
///     <br />
///     @lgr: check if fully replaceable with AutoMapper
/// </summary>
/// <typeparam name="TDto"></typeparam>
/// <typeparam name="TPeer"></typeparam>
public abstract class DataTransferObjectMapperBase<TDto, TPeer>
{
    private readonly DataTransferObjectMappings<TDto, TPeer> _mappings;

    /// <summary>
    ///     .ctor
    /// </summary>
    protected DataTransferObjectMapperBase()
    {
        var mappings = new DataTransferObjectMappings<TDto, TPeer>();
        // ReSharper disable once VirtualMemberCallInConstructor
        InitMappings(mappings);
        _mappings = mappings;
    }

    /// <summary>
    ///     Method is overridden by subclasses to initialize the mappings.
    /// </summary>
    /// <param name="mappings"></param>
    protected abstract void InitMappings(DataTransferObjectMappings<TDto, TPeer> mappings);


    /// <summary>
    ///     Applies the mapping from the source to the data object.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dataObject"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    protected bool ToDto(TPeer source, TDto dataObject)
    {
        return _mappings.ToDto(source ?? throw new ArgumentNullException(nameof(source)), dataObject);
    }

    /// <summary>
    ///     Applies the mapping from the data object to the target.
    /// </summary>
    /// <param name="dataObject"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    protected bool FromDto(TDto dataObject, TPeer target)
    {
        return _mappings.FromDto(dataObject ?? throw new ArgumentNullException(nameof(dataObject)),
            target);
    }
}