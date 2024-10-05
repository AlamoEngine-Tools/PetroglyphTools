using System;

namespace PG.Commons.Data;

internal class DataTransferTransferObjectMapping<TDto, TPeer, TValue>(
    Func<TDto, TValue> dataObjectValueGetter,
    Action<TDto, TValue> dataObjectValueSetter,
    Func<TPeer, TValue> peerValueGetter,
    Action<TPeer, TValue> peerValueSetter)
    : IDataTransferObjectMapping<TDto, TPeer>
{
    private Func<TDto, TValue> DataObjectValueGetter { get; } = dataObjectValueGetter;
    private Action<TDto, TValue> DataObjectValueSetter { get; } = dataObjectValueSetter;
    private Func<TPeer, TValue> PeerValueGetter { get; } = peerValueGetter;
    private Action<TPeer, TValue> PeerValueSetter { get; } = peerValueSetter;

    public void ToDto(TPeer source, TDto dataObject)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (dataObject == null) throw new ArgumentNullException(nameof(dataObject));

        var value = PeerValueGetter.Invoke(source);
        DataObjectValueSetter.Invoke(dataObject, value);
    }

    public void FromDto(TDto dataObject, TPeer target)
    {
        if (target == null) throw new ArgumentNullException(nameof(target));
        if (dataObject == null) throw new ArgumentNullException(nameof(dataObject));
        var value = DataObjectValueGetter.Invoke(dataObject);
        PeerValueSetter.Invoke(target, value);
    }
}