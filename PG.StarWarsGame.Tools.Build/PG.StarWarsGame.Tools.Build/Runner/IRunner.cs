using PG.Commons.Environment;

namespace PG.StarWarsGame.Tools.Build.Runner
{
    public interface IRunner
    {
        ExitCode Run();
    }
}