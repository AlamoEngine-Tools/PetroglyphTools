using System;

namespace PG.Commons.Data;

internal class DataObjectMapping<TDataObject, TPeer, TValue>(
    Func<TDataObject, TValue> dataObjectValueGetter,
    Action<TDataObject, TValue> dataObjectValueSetter,
    Func<TPeer, TValue> peerValueGetter,
    Action<TPeer, TValue> peerValueSetter)
    : IDataObjectMapping<TDataObject, TPeer>
{
    private Func<TDataObject, TValue> DataObjectValueGetter { get; } = dataObjectValueGetter;
    private Action<TDataObject, TValue> DataObjectValueSetter { get; } = dataObjectValueSetter;
    private Func<TPeer, TValue> PeerValueGetter { get; } = peerValueGetter;
    private Action<TPeer, TValue> PeerValueSetter { get; } = peerValueSetter;

    public void ToDataObject(TPeer source, TDataObject dataObject)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (dataObject == null) throw new ArgumentNullException(nameof(dataObject));

        var value = PeerValueGetter.Invoke(source);
        DataObjectValueSetter.Invoke(dataObject, value);
    }

    public void FromDataObject(TDataObject dataObject, TPeer target)
    {
        if (target == null) throw new ArgumentNullException(nameof(target));
        if (dataObject == null) throw new ArgumentNullException(nameof(dataObject));
        var value = DataObjectValueGetter.Invoke(dataObject);
        PeerValueSetter.Invoke(target, value);
    }
}