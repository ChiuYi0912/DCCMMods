using System.Reflection;
using System.Runtime.CompilerServices;

namespace MoreSettings;

internal static class ModuleInit
{
    [ModuleInitializer]
    public static void Initialize()
    {
        var modDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

        foreach (var subDir in Directory.GetDirectories(modDir))
        {
            foreach (var dll in Directory.GetFiles(subDir, "*.dll"))
            {
                try
                {
                    Assembly.LoadFrom(dll);
                }
                catch{}
            }
        }
    }
}
