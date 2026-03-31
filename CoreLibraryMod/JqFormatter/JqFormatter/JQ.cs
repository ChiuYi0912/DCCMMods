using System.Diagnostics;
using System.Reflection;
using System.Text;
using dc;
using dc.haxe.format;
using dc.hxd;
using JqFormatter.Config;
using ModCore.Events.Interfaces.Game;
using ModCore.Events.Interfaces.Game.Hero;
using ModCore.Mods;
using ModCore.Storage;
using ModCore.Utilities;
using Serilog;

namespace JqFormatter;

public class JQ(ModInfo info) : ModBase(info)
{
  public static Config<ModConfig> config = new("JqFormatterConfig");
  public override void Initialize(){}

  public static string FormatJson(string json)
  {
    string assemblyPath = Assembly.GetExecutingAssembly().Location;

    var startInfo = new ProcessStartInfo
    {
      RedirectStandardInput = true,
      RedirectStandardOutput = true,
      RedirectStandardError = true,
      UseShellExecute = false,
      CreateNoWindow = true,
      StandardInputEncoding = Encoding.UTF8,
      StandardOutputEncoding = Encoding.UTF8,
      StandardErrorEncoding = Encoding.UTF8
    };

    var process = WorkerProcessUtils.StartWorkerProcess(
         typeof(JqWorker).AssemblyQualifiedName!,
        "WorkerEntry",
        startInfo,
        assemblyPath
    );
    process.StandardInput.Write(json);
    process.StandardInput.Close();


    string result = process.StandardOutput.ReadToEnd();
    string error = process.StandardError.ReadToEnd();

    process.WaitForExit(5000);

    if (!string.IsNullOrEmpty(error))
      throw new InvalidOperationException($"jq错误: {error}");

    return result;

  }
}
