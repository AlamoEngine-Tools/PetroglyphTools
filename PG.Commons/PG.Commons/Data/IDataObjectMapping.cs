namespace PG.Commons.Data;

internal interface IDataObjectMapping<in TDataObject, in TPeer>
{
    internal void ToDataObject(TPeer source, TDataObject dataObject);

    internal void FromDataObject(TDataObject dataObject, TPeer target);
}