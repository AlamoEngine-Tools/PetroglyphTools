using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PG.Commons.Data;

/// <summary>
///     Data mappings.
/// </summary>
/// <typeparam name="TDataObject"></typeparam>
/// <typeparam name="TPeer"></typeparam>
public class DataObjectMappings<TDataObject, TPeer>
{
    private List<IDataObjectMapping<TDataObject, TPeer>> Mappings { get; } = new();

    // ReSharper disable once HeapView.ClosureAllocation
    internal bool ToDataObject([DisallowNull] TPeer source, TDataObject dataObject)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        try
        {
            // ReSharper disable once HeapView.DelegateAllocation
            Mappings.ForEach(m => m.ToDataObject(source, dataObject));
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    // ReSharper disable once HeapView.ClosureAllocation
    internal bool FromDataObject([DisallowNull] TDataObject dataObject, TPeer target)
    {
        if (dataObject == null) throw new ArgumentNullException(nameof(dataObject));
        try
        {
            // ReSharper disable once HeapView.DelegateAllocation
            Mappings.ForEach(m => m.FromDataObject(dataObject, target));
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
    public DataObjectMappings<TDataObject, TPeer> With<TValue>(Func<TDataObject, TValue> dataObjectValueGetter,
        Action<TDataObject, TValue> dataObjectValueSetter,
        Func<TPeer, TValue> peerValueGetter, Action<TPeer, TValue> peerValueSetter)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        Mappings.Add(new DataObjectMapping<TDataObject, TPeer, TValue>(dataObjectValueGetter, dataObjectValueSetter,
            peerValueGetter, peerValueSetter));
        return this;
    }
}