using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PG.Commons.Data;

/// <summary>
///     Data mappings.
/// </summary>
/// <typeparam name="TDto"></typeparam>
/// <typeparam name="TPeer"></typeparam>
public class DataTransferObjectMappings<TDto, TPeer>
{
    private List<IDataTransferObjectMapping<TDto, TPeer>> Mappings { get; } = new();

    // ReSharper disable once HeapView.ClosureAllocation
    internal bool ToDto([DisallowNull] TPeer source, TDto dataObject)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        try
        {
            // ReSharper disable once HeapView.DelegateAllocation
            Mappings.ForEach(m => m.ToDto(source, dataObject));
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    // ReSharper disable once HeapView.ClosureAllocation
    internal bool FromDto([DisallowNull] TDto dataObject, TPeer target)
    {
        if (dataObject == null) throw new ArgumentNullException(nameof(dataObject));
        try
        {
            // ReSharper disable once HeapView.DelegateAllocation
            Mappings.ForEach(m => m.FromDto(dataObject, target));
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    ///     Mapping builder.
    /// </summary>
    /// <param name="dataObjectValueGetter"></param>
    /// <param name="dataObjectValueSetter"></param>
    /// <param name="peerValueGetter"></param>
    /// <param name="peerValueSetter"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public DataTransferObjectMappings<TDto, TPeer> With<TValue>(Func<TDto, TValue> dataObjectValueGetter,
        Action<TDto, TValue> dataObjectValueSetter,
        Func<TPeer, TValue> peerValueGetter, Action<TPeer, TValue> peerValueSetter)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        Mappings.Add(new DataTransferTransferObjectMapping<TDto, TPeer, TValue>(dataObjectValueGetter,
            dataObjectValueSetter,
            peerValueGetter, peerValueSetter));
        return this;
    }
}