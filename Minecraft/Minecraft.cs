using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.ProcessBuilder;

namespace Minecraft;

public class Minecraft
{
    private readonly MinecraftLauncher _minecraftLauncher;
    public Minecraft()
    {
        var path = MinecraftPath.GetOSDefaultPath();
        _minecraftLauncher = new MinecraftLauncher(path);
        _minecraftLauncher.FileProgressChanged += (sender, args) =>
        {
            LauncherLogger.LogInfo<Minecraft>($"Downloaded: {args.Name}");
        };
    }
    public async Task Play()
    {
        LauncherLogger.LogInfo<Minecraft>("Starting minecraft.");
        var session = MSession.CreateOfflineSession("xygoodlol");
        var process = await _minecraftLauncher.BuildProcessAsync("1.21.4", new MLaunchOption()
        {
            Session = session,
            MaximumRamMb = 4000,
            FullScreen = false,
            JavaPath = "C:\\Users\\30958\\AppData\\Roaming\\.minecraft\\runtime\\java-runtime-delta\\bin\\javaw.exe"
        });
        var wrapper=  new ProcessWrapper(process);
        wrapper.OutputReceived += ((sender, s) =>
        {
            LauncherLogger.LogInfo<Minecraft>(s);
        });
        wrapper.Exited += (sender, args) =>
        {
            LauncherLogger.LogInfo<global::Minecraft.Minecraft>("Game exited. ");
        };
        wrapper.StartWithEvents();
        var exitcode = await wrapper.WaitForExitTaskAsync();
        LauncherLogger.LogInfo<Minecraft>($"Exit code: {exitcode}");
    }
}