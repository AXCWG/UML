using System.Diagnostics;
using AXExpansion;

namespace Minecraft;

public static class LauncherLogger
{
    private static StreamWriter _logStreamWriter;
    static LauncherLogger()
    {
        Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            .PathJoin("AXCWG", "UML", "log"));
        var latest = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            .PathJoin("AXCWG", "UML", "log", "latest.log");
        
        if (File.Exists(latest))
        {
            File.Move(latest,Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                .PathJoin("AXCWG", "UML", "log", $"log_{File.GetLastWriteTime(latest).ToString("O").Replace(':', '-')}.log") );
        }
        var logFileStream = new FileStream(latest, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        _logStreamWriter = new StreamWriter(logFileStream);
    }
    
    public static void Log<T>(string message)
    {
        Log(message, typeof(T).Name, "LOG");
    }

    public static void Log(string message, string? typeName = null)
    {
        Log(message, typeName, "LOG");
    }

    public static void LogInfo(string message, string? typeName = null)
    {
        Log(message, typeName, "INFO");
    }

    public static void LogInfo<T>(string message)
    {
        Log(message, typeof(T).Name, "INFO");
    }

    public static void LogWarning(string message, string? typeName = null)
    {
        Log(message, typeName, "WARNING");
    }

    public static void LogWarning<T>(string message)
    {
        Log(message, typeof(T).Name, "WARNING");
    }

    public static void LogError(string message, string? typeName = null)
    {
        Log(message, typeName, "ERROR");
    }

    public static void LogError<T>(string message)
    {
        Log(message, typeof(T).Name, "ERROR");
    }

    private static void Log(string message, string? type, string level)
    {
        var format = string.Format("[{0}] [{1}/{2}]: {3}", DateTime.Now, type ?? "Unknown caller", level,
            message);
        Debug.WriteLine(format);
        Console.WriteLine(format);
        var textWriter = TextWriter.Synchronized(_logStreamWriter);
        textWriter.WriteLine(format);
        textWriter.Flush();
    }
}