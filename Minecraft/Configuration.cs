using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text.Json;
using AXExpansion;
using CmlLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace Minecraft;

public static class State 
{
    internal static  Minecraft LauncherBackend { get; set; } = new Minecraft();
    
    private static  JsonSerializerOptions Options => new JsonSerializerOptions(JsonSerializerOptions.Default).With(d =>
    {
        d.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        d.WriteIndented = true;
    });
    static State()
    {
        Validation();
    }

    public static void Validation()
    {
        var pathJoin = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            .PathJoin("AXCWG", "UML", "state.json");
        if (!File.Exists(pathJoin))
        {
            LauncherLogger.LogWarning("No state file found at default directory. Making one now...", nameof(State));
            File.WriteAllText(pathJoin, JsonSerializer.Serialize(new StateSnapshot()
            {
                Online = false, Profiles = [new()
                {
                    Path = MinecraftPath.GetOSDefaultPath(), Name = "Default Official Profile"
                }]
            }, options: Options));return;
        }

            
        LauncherLogger.LogInfo("Validating state...", nameof(State));
        try
        {
            var test = JsonSerializer.Deserialize<StateSnapshot>(File.ReadAllText(pathJoin), Options);
            if (test is null)
            {
                LauncherLogger.LogError("Whole file is null. Deleting existing state file and replace with a new one.",
                    nameof(State));
                File.Delete(pathJoin);
                Validation();
                return;
            }

            if (test.Online is null)
            {
                LauncherLogger.LogError(
                    "Property 'online' is not present/is null in file. Defaulting back to false...");
                test.Online = false;
            }

            if (test.Profiles is null)
            {
                LauncherLogger.LogError("Property 'profiles' is not present/is null in file. Defaulting back to [OSDefault]...");
                test.Profiles =
                [
                    new()
                    {
                        Path = MinecraftPath.GetOSDefaultPath(),
                        Name = "Default Official Profile"
                    }
                ];
            }

           
            for (var i = 0; i < test.Profiles.Count; i++)
            {
                if (test.Profiles[i].Path is null)
                {
                    LauncherLogger.LogWarning("Profile found abnormal entry: path is null. Removing and continue.", nameof(State));
                    test.Profiles.RemoveAt(i);
                    i--;
                    continue;
                }
                if (test.Profiles[i].Name is null)
                {
                     LauncherLogger.LogError("Profile found abnormal entry: name is null. Renaming to default and continue. ", nameof(State));
                     test.Profiles[i].Name = "Unnamed";
                }

                
            }

            File.WriteAllText(pathJoin, JsonSerializer.Serialize(test, Options));
            LauncherLogger.LogInfo("Validated. ", nameof(State));
        }
        catch (JsonException e)
        {
            LauncherLogger.LogError("Validation failed. Deleting existing state file and replace with a new one.", nameof(State));
            File.Delete(pathJoin);
            Validation();
            return;
        }
       
    }

    [Obsolete("Nobody uses it. ")]
    public static void Set<T, TProp>(Expression<Func<StateSnapshot, TProp>> expression, T value)
    {
        if (expression.Body is MemberExpression expressionMember)
        {
            if (expressionMember.Member.MemberType is MemberTypes.Property)
            {
                LauncherLogger.LogInfo($"Setting state property \"{expressionMember.Member.Name}\"");
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
                            .PathJoin("AXCWG", "UML", "state.json"),JsonSerializer.Serialize<StateSnapshot>(obj, Options));
                    }
                    else
                    {
                        throw new MissingMemberException(className: nameof(StateSnapshot), memberName: expressionMember.Member.Name);
                    }
                }
                catch (JsonException e)
                {
                    LauncherLogger.LogError($"Error setting property: \n{e}", nameof(State));
                }
                catch (MissingMemberException e)
                {
                    LauncherLogger.LogError($"State property {expressionMember.Member.Name} does not exists. \n{e}");
                }
              
            }
        }
    }
    
    public static void Set(StateSnapshot obj)
    {
        Validation();
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
                JsonSerializer.Serialize<StateSnapshot>(stored ?? throw new NullReferenceException(), Options));
        }
        catch (JsonException e)
        {
            LauncherLogger.LogError($"Error setting property: \n{e}", nameof(State));
        }
        catch (InvalidOperationException e)
        {
            LauncherLogger.LogError("Something really wrong occured to dotnet runtime. ");
            throw new ApplicationException("Something really wrong occured to dotnet runtime. ");
        }
        catch (NullReferenceException)
        {
            Validation();
            Set(obj);
        }
        // Set(i=>i.Online, obj.Online);
    }
    
    public static T? Get<T>(Expression<Action<StateSnapshot>> expression)
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

            return (T?)prop.GetValue(obj);
        }
        catch (MemberAccessException e)
        {
            LauncherLogger.LogError($"Get error: {e}", nameof(State));
        }
        throw new Exception("Get error");
        
    }
    public static StateSnapshot GetAll =>
        JsonSerializer.Deserialize<StateSnapshot>(File.ReadAllText(Environment
            .GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
            .PathJoin("AXCWG", "UML", "state.json")), Options) ?? throw new NullReferenceException();
}

public class StateSnapshot
{
    public required bool? Online { get; set; } = false;
    public required List<Profile>? Profiles { get; set; }
    
    public class Profile
    {
        public required string? Path { get; set; }
        public required string? Name { get; set; }
    }
    
}