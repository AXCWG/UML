using System.IO.Compression;
using System.Reflection;

namespace Minecraft;
public static class ResourceExtractor
{
    private static string? _tempDir;
    [STAThread]
    public static string Extract()
    {
        var assembly = Assembly.GetExecutingAssembly();
        const string resourceName = "ui.wwwroot.zip";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
            throw new InvalidOperationException(
                $"Embedded resource '{resourceName}' not found. Ensure the frontend is built (npm run build) before publishing.");

        _tempDir = Path.Combine(Path.GetTempPath(), "MinecraftLauncher", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);

        using var archive = new ZipArchive(stream, ZipArchiveMode.Read);
        archive.ExtractToDirectory(_tempDir, overwriteFiles: true);

        return Path.Combine(_tempDir, "index.html");
    }
    [STAThread]
    public static void Cleanup()
    {
        if (_tempDir != null && Directory.Exists(_tempDir))
        {
            try { Directory.Delete(_tempDir, recursive: true); }
            catch { }
        }

        var launcherTemp = Path.Combine(Path.GetTempPath(), "MinecraftLauncher");
        if (Directory.Exists(launcherTemp))
        {
            try
            {
                foreach (var dir in Directory.GetDirectories(launcherTemp))
                {
                    try { Directory.Delete(dir, recursive: true); }
                    catch { }
                }
            }
            catch { }
        }
    }
}
