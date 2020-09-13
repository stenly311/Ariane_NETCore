using System.IO;
using System.Reflection;

namespace Ariane
{
    internal class Globals
    {
        public static string JsonFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Configuration.json");
    }
}
