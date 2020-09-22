using System.IO;
using System.Linq;
using System.Reflection;

namespace PG.Commons.Util
{
    public static class EmbeddedResourceUtility
    {
        private static readonly Assembly ASSEMBLY = Assembly.GetExecutingAssembly();

        public static string GetResourceByFileName(string name)
        {
            return ASSEMBLY.GetManifestResourceNames()
                .Single(str => str.EndsWith(name));
        }

        public static Stream GetResourceAsStreamByFileName(string name)
        {
            return ASSEMBLY.GetManifestResourceStream(GetResourceByFileName(name));
        }
    }
}