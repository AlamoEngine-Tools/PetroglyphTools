using System;

namespace PG.Commons.Data;

/// <summary>
///     Container class allowing to map from a data objct to a peer and vice versa
/// </summary>
/// <typeparam name="TDataObject"></typeparam>
/// <typeparam name="TPeer"></typeparam>
public abstract class DataObjectMapperBase<TDataObject, TPeer>
{
    private readonly DataObjectMappings<TDataObject, TPeer> _mappings;

    /// <summary>
    ///     .ctor
    /// </summary>
    protected DataObjectMapperBase()
    {
        var mappings = new DataObjectMappings<TDataObject, TPeer>();
        // ReSharper disable once VirtualMemberCallInConstructor
        InitMappings(mappings);
        _mappings = mappings;
    }

    /// <summary>
    ///     Method is overridden by subclasses to initialize the mappings.
    /// </summary>
    /// <param name="mappings"></param>
    protected abstract void InitMappings(DataObjectMappings<TDataObject, TPeer> mappings);


    /// <summary>
    ///     Applies the mapping from the source to the data object.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="dataObject"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    protected bool ToDataObject(TPeer source, TDataObject dataObject)
    {
        return _mappings.ToDataObject(source ?? throw new ArgumentNullException(nameof(source)), dataObject);
    }

    /// <summary>
    ///     Applies the mapping from the data object to the target.
    /// </summary>
    /// <param name="dataObject"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    protected bool FromDataObject(TDataObject dataObject, TPeer target)
    {
        return _mappings.FromDataObject(dataObject ?? throw new ArgumentNullException(nameof(dataObject)),
            target);
    }
}