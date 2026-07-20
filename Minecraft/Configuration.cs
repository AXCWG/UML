using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using AXExpansion;
using CmlLib.Core;
using CmlLib.Core.ProcessBuilder;
using CmlLib.Core.Version;
using CmlLib.Core.VersionLoader;
using Log = Minecraft.LauncherLogger;
// ReSharper disable VariableHidesOuterVariable

namespace Minecraft;

public static class State 
{
    internal static  Minecraft LauncherBackend { get; set; } = new Minecraft();
    
    private static  JsonSerializerOptions Options => new JsonSerializerOptions(JsonSerializerOptions.Default).With(d =>
    {
        d.WriteIndented = true;
        d.AllowTrailingCommas = true;
    });
    static State()
    {
        Validation();
    }
    // public static IEnumerable<StateSnapshot.VersionConfig> EnumeratePossibleLocalVersion(string path)
    // {
    //                 
    //     var enumerateDirectories = Directory.EnumerateDirectories(path.PathJoin("versions"));
    //     foreach (var enumerateDirectory in enumerateDirectories)
    //     {
    //         Console.WriteLine($"Found {Path.GetFileName(enumerateDirectory)}");
    //         yield return new()
    //         {
    //             Name = Path.GetFileName(enumerateDirectory),
    //             JrePath = null,
    //             MemMegabyte = null
    //         };
    //     }
    // }
    private static MinecraftLauncher ConvieniantlyGetMLauncher(string path) => new MinecraftLauncher(MinecraftLauncherParameters.CreateDefault().With(i =>
    {
        i.VersionLoader =  new LocalJsonVersionLoader(new MinecraftPath(path));
            
    }));
    public static async Task<int> CheckVersion()
    {
        while (true)
        {
            var profiles = Get(i => i.Profiles);
            if (profiles is null || profiles.Any(i => i.UnreliableVersionList is null) || profiles.Any(i => i.Path is null))
            {
                Log.LogError("Check version failed. Rerunning validation then recursive execution. ", nameof(CheckVersion));
                Validation();
                continue; 
            }

            var count = 0;
            if (true)// TODO
            {
                foreach (var profile in profiles)
                {
                    var l = await ConvieniantlyGetMLauncher(profile.Path!).GetAllVersionsAsync();
                    
                    profile.UnreliableVersionList!.Remove(i =>
                    {
                        if (!l.Contains(i.Value.Name))
                        {
                            count++;
                            return true;
                        }

                        return false; 
                    });
                    var existButNoRecord =
                        l.Where(i => profile.UnreliableVersionList!.All(o => o.Value.Name != i.Name)).ToList();
                    foreach (var versionMetadata in existButNoRecord)
                    {
                        profile.UnreliableVersionList!.Add(Guid.NewGuid().ToString(), new StateSnapshot.VersionConfig()
                        {
                            Name = versionMetadata.Name
                        });
                    }
                }

               
            }
            else
            {
                // TODO Possibly
                // profiles.FirstOrDefault(i=>i.Uuid == uuid)?.UnreliableVersionList!.Remove(i =>
                // {
                //     if (!l.Contains(i.Value.Name))
                //     {
                //         count++;
                //         return true;
                //     }
                //
                //     return false; 
                // });
            }
            
            Set(i=>i.Profiles, profiles);
            return count;
        }
       
    }
    // public static int CheckVersion(string? uuid = null)
    // {
    //     while (true)
    //     {
    //         var profiles = Get(i => i.Profiles);
    //         if (profiles is null || profiles.Any(i => i.UnreliableVersionList is null) || profiles.Any(i => i.Path is null))
    //         {
    //             Log.LogError("Check version failed. Rerunning validation then recursive execution. ", nameof(CheckVersion));
    //             Validation();
    //             continue;
    //         }
    //
    //         int count = 0;
    //         if (uuid is null)
    //         {
    //             foreach (var profile in profiles)
    //             {
    //                 var enumeratePossibleLocalVersion = EnumeratePossibleLocalVersion(profile.Path!).ToList();
    //             
    //                 foreach (var versionConfig in enumeratePossibleLocalVersion)
    //                 {
    //                     if (profile.UnreliableVersionList!.Any(i => i.Value.Name == versionConfig.Name))
    //                     {
    //                         continue;
    //                     }
    //                     profile.UnreliableVersionList!.Add(Guid.NewGuid().ToString(),  versionConfig);
    //                     count++; 
    //                 }
    //                 var list = profile.UnreliableVersionList!.ToList();
    //                 foreach (var t in list)
    //                 {
    //                     if (enumeratePossibleLocalVersion.Any(vi => vi.Name == t.Value.Name))
    //                         continue;
    //                     profile.UnreliableVersionList!.Remove(t.Key);
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             var profile = profiles.FirstOrDefault(i=>i.Uuid == uuid);
    //             if (profile is null)
    //             {
    //                 throw new IOException("Check version uuid not exist. ");
    //             }
    //             var enumeratePossibleLocalVersion = EnumeratePossibleLocalVersion(profile.Path!).ToList();
    //             
    //             foreach (var versionConfig in enumeratePossibleLocalVersion)
    //             {
    //                 if (profile.UnreliableVersionList!.Any(i => i.Value.Name == versionConfig.Name))
    //                 {
    //                     continue;
    //                 }
    //                 profile.UnreliableVersionList!.Add(Guid.NewGuid().ToString(),  versionConfig);
    //                 count++; 
    //             }
    //             var list = profile.UnreliableVersionList!.ToList();
    //             foreach (var t in list)
    //             {
    //                 if (enumeratePossibleLocalVersion.Any(vi => vi.Name == t.Value.Name))
    //                     continue;
    //                 profile.UnreliableVersionList!.Remove(t.Key);
    //             }
    //         }
    //         
    //         return count;
    //         
    //     }
    // }

    public static async  void A()
    {
        var m = new MinecraftLauncher(MinecraftLauncherParameters.CreateDefault().With(i =>
        {
            i.VersionLoader =  new LocalJsonVersionLoader(new MinecraftPath(MinecraftPath.GetOSDefaultPath()));
            
        }));
    }
    public static void Validation()
    {
        var pathJoin = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            .PathJoin("AXCWG", "UML", "state.json");
        if (!File.Exists(pathJoin))
        {
            Log.LogWarning("No state file found at default directory. Making one now...", nameof(State));
            var uuid = Guid.NewGuid().ToString();
            File.WriteAllText(pathJoin, JsonSerializer.Serialize(new StateSnapshot()
            {
                Online = false, Profiles = [new()
                {
                    Path = MinecraftPath.GetOSDefaultPath(), Name = "Default Official Profile", Uuid = uuid,
                }], SelectedProfileUuid = uuid
            }, options: Options));return;
        }

            
        Log.LogInfo("Validating state...", nameof(State));
        try
        {
            // TODO on adding property
            var test = JsonSerializer.Deserialize<StateSnapshot>(File.ReadAllText(pathJoin), Options);
            if (test is null)
            {
                Log.LogError("Whole file is null. Deleting existing state file and replace with a new one.",
                    nameof(State));
                File.Delete(pathJoin);
                Validation();
                return;
            }

            if (test.Online is null)
            {
                Log.LogError(
                    "Property 'online' is not present/is null in file. Defaulting back to false...");
                test.Online = false;
            }

            if (test.Profiles is null)
            {
                Log.LogError("Property 'profiles' is not present/is null in file. Defaulting back to [OSDefault]...");
                test.Profiles =
                [
                    new()
                    {
                        Path = MinecraftPath.GetOSDefaultPath(),
                        Name = "Default Official Profile",
                        Uuid = Guid.NewGuid().ToString(),
                    }
                ];
            }

            if (test.JrePath is null)
            {
                //TODO
            }

            if (test.MemMegabyte is null)
            {
                Log.LogError($"Property '{nameof(test.MemMegabyte)}' is not present or is null in file. Defaulting back to 1024.");
                test.MemMegabyte = 1024;
            }

            

            for (var i = 0; i < test.Profiles.Count; i++)
            {
                if (test.Profiles[i].Path is null)
                {
                    Log.LogWarning($"Profile found abnormal entry: {nameof(StateSnapshot.Profile.Path)} is null. Removing and continue.", nameof(State));
                    test.Profiles.RemoveAt(i);
                    i--;
                    continue;
                }
                if (test.Profiles[i].Name is null)
                {
                     Log.LogError($"Profile found abnormal entry: {nameof(StateSnapshot.Profile.Name)} is null. Renaming to default and continue. ", nameof(State));
                     test.Profiles[i].Name = "Unnamed";
                }

                if (test.Profiles[i].Uuid is null)
                {
                    Log.LogError($"Profile found abnormal entry: {nameof(StateSnapshot.Profile.Uuid)} is null. Automatically applying for it. ", nameof(State));
                    test.Profiles[i].Uuid = Guid.NewGuid().ToString();
                }

                if (test.Profiles[i].UnreliableVersionList is null)
                {
                    Log.LogError($"Property '{nameof(StateSnapshot.Profile.UnreliableVersionList)}' is null or not presented. Now checking existing versions. ", nameof(State));
                    var minecraftPath = new MinecraftPath(MinecraftPath.GetOSDefaultPath());
                    var exist = new MinecraftLauncher(MinecraftLauncherParameters.CreateDefault(minecraftPath)
                        .With(i =>
                        {
                            i.VersionLoader = new LocalJsonVersionLoader(minecraftPath);
                        })).GetAllVersionsAsync().GetAwaiter().GetResult();
                    test.Profiles[i].UnreliableVersionList = new Dictionary<string, StateSnapshot.VersionConfig>().With(d =>
                    {
                        foreach (var versionConfig in exist)
                        {
                            Log.LogInfo($"Adding {versionConfig.Name}");
                            d.Add(Guid.NewGuid().ToString(), new()
                            {
                                Name = versionConfig.Name,
                                JrePath = null,
                                MemMegabyte = null
                            });
                        }
                    });
                }

                test.Profiles[i].UnreliableVersionList!.Remove(i =>
                {
                    if (i.Value.Name is not null) return false;
                    Log.LogError("Found incomplete version entry with null as name. Prepare to remove. ");
                    return true;
                });
                

            }

            if (test.SelectedProfileUuid is null && test.Profiles.Count != 0)
            {
                Log.LogError($"Property '{nameof(test.SelectedProfileUuid)}' is not present/is null in file. Defaulting back to default...");
                test.SelectedProfileUuid = test.Profiles[0].Uuid;
            }

            if (test.OfflineNameList is null || (test.Selected is null && test.OfflineNameList is null))
            {
                Log.LogError($"Property '{nameof(test.OfflineNameList)}' or '{nameof(test.Selected)}' is null. Rebuilding. ");
                test.OfflineNameList = [];
                test.Selected = null; 
            }
            File.WriteAllText(pathJoin, JsonSerializer.Serialize(test, Options));
            CheckVersion().GetAwaiter().GetResult();
            
            Log.LogInfo("Validated. ", nameof(State));
        }
        catch (JsonException e)
        {
            Log.LogError(e.ToString(), nameof(State));
            Log.LogError("Validation failed. Deleting existing state file and replace with a new one.", nameof(State));
            File.Delete(pathJoin);
            Validation();
        }
       
    }

    [Obsolete("Nobody uses it. ")]
    public static void Set<T, TProp>(Expression<Func<StateSnapshot, TProp>> expression, T value)
    {
        if (expression.Body is MemberExpression expressionMember)
        {
            if (expressionMember.Member.MemberType is MemberTypes.Property)
            {
                Log.LogInfo($"Setting state property \"{expressionMember.Member.Name}\"", nameof(State));
                try
                {
                    var obj = JsonSerializer.Deserialize<StateSnapshot>(File.ReadAllText(Environment
                        .GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                        .PathJoin("AXCWG", "UML", "state.json")), Options);
                    if (obj is null)
                    {
                        throw new JsonException("Whole file is null. ");
                    }

                    if (typeof(StateSnapshot).GetProperty(expressionMember.Member.Name) is { } property)
                    {
                        property.SetValue(obj, value);
                        File.WriteAllText(Environment
                            .GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                            .PathJoin("AXCWG", "UML", "state.json"),JsonSerializer.Serialize(obj, Options));
                    }
                    else
                    {
                        throw new MissingMemberException(className: nameof(StateSnapshot), memberName: expressionMember.Member.Name);
                    }
                }
                catch (JsonException e)
                {
                    Log.LogError($"Error setting property: \n{e}", nameof(State));
                }
                catch (MissingMemberException e)
                {
                    Log.LogError($"State property {expressionMember.Member.Name} does not exists. \n{e}");
                }
              
            }
        }
    }
    
    public static void Set(StateSnapshot obj)
    {
        try
        {
            var stored = JsonSerializer.Deserialize<StateSnapshot>(File.ReadAllText(Environment
                .GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                .PathJoin("AXCWG", "UML", "state.json")), Options);

            foreach (var propertyInfo in typeof(StateSnapshot).GetProperties(
                         BindingFlags.Public | BindingFlags.Instance))
            {
                propertyInfo.SetValue(stored,
                    (typeof(StateSnapshot).GetProperty(propertyInfo.Name) ?? throw new InvalidOperationException())
                    .GetValue(obj));
            }

            File.WriteAllText(Environment
                    .GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                    .PathJoin("AXCWG", "UML", "state.json"),
                JsonSerializer.Serialize(stored ?? throw new NullReferenceException(), Options));
        }
        catch (JsonException e)
        {
            Log.LogError($"Error setting property: \n{e}", nameof(State));
        }
        catch (InvalidOperationException)
        {
            Log.LogError("Something really wrong occured to dotnet runtime. ");
            throw new ApplicationException("Something really wrong occured to dotnet runtime. ");
        }
        catch (NullReferenceException)
        {
            Validation();
            Set(obj);
        }
        // Set(i=>i.Online, obj.Online);
    }
    
    public static T Get<T>(Expression<Func<StateSnapshot, T>> expression)
    {
        try
        {
            var obj = JsonSerializer.Deserialize<StateSnapshot>(File.ReadAllText(Environment
                .GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                .PathJoin("AXCWG", "UML", "state.json")), Options);
            if (expression.Body is not MemberExpression expressionMember || expressionMember.Member.MemberType is not MemberTypes.Property)
                throw new MemberAccessException("Only property can be accessed through this method. ");
            var prop = (typeof(StateSnapshot).GetProperty(expressionMember.Member.Name));
            if (prop is null)
            {
                throw new MissingMemberException(className: nameof(StateSnapshot),
                    memberName: expressionMember.Member.Name);
            }

            var s = (T?)prop.GetValue(obj);
            if (s is null)
            {
                Log.LogError($"Get error. Triggering {nameof(Validation)}.");
                Validation();
                s = Get(expression);
            }

            return s;
        }
        catch (MemberAccessException e)
        {
            Log.LogError($"Get error: {e}\nTrying to revalidate. ", nameof(State));
            Validation();
            return Get(expression);
        }
        
    }
    public static StateSnapshot GetAll
    {
        get
        {
            return JsonSerializer.Deserialize<StateSnapshot>(File.ReadAllText(Environment
                .GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                .PathJoin("AXCWG", "UML", "state.json")), Options) ?? throw new NullReferenceException();
        }
    }
}

public class StateSnapshot : IVersionConfig
{
    public  bool? Online { get; set; } = false;
    public  List<Profile>? Profiles { get; set; }
    
    public class Profile
    {
        public  string? Path { get; set; }
        public  string? Name { get; set; }
        public  string? Uuid { get; set; }
        public Dictionary<string, VersionConfig>? UnreliableVersionList { get; set; }
    }
    // TODO
    public class VersionConfig : IVersionConfig
    {
        public string? Name { get; set; }
        public string? Version { get; set; }
        public string? JrePath { get; set; }
        public long? MemMegabyte { get; set; }
    }
    public string? SelectedProfileUuid { get; set; }

    public string? JrePath { get; set; }
    public long? MemMegabyte { get; set; }

    public List<string>? OfflineNameList { get; set; }
    [AllowNull]
    public int? Selected { get; set; }
}
public interface IVersionConfig
{
    public string? JrePath { get; set; }
    public long? MemMegabyte { get; set; }
}