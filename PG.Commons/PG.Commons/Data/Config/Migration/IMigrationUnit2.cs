using PG.Commons.Environment;

namespace PG.Commons.Data.Config.Migration
{
    public interface IMigrationUnit2<T1,T2> : IMigrationUnit
    {
        T1 MigrateTo(T2 fromType);
        T2 MigrateTo(T1 fromType);
    }
}