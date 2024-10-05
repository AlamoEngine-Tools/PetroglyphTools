namespace PG.Commons.Data;

internal interface IDataTransferObjectMapping<in TDataObject, in TPeer>
{
    internal void ToDto(TPeer source, TDataObject dataObject);

    internal void FromDto(TDataObject dataObject, TPeer target);
}