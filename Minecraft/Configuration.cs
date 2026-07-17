using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text.Json;
using AXExpansion;

namespace Minecraft;

public static class State 
{
    private static  JsonSerializerOptions Options => new JsonSerializerOptions(JsonSerializerOptions.Default).With(d =>
    {
        d.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        d.WriteIndented = true;
    });
    static State()
    {
        void Recursive()
        {
            var pathJoin = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                .PathJoin("AXCWG", "UML", "state.json");
            if (!File.Exists(pathJoin))
            {
                LauncherLogger.LogWarning("No state file found at default directory. Making one now...", nameof(State));
                File.WriteAllText(pathJoin, JsonSerializer.Serialize(new StateSnapshot()
                {
                    Online = false
                }, options: Options));return;
            }

            try
            {
                LauncherLogger.LogInfo("Validating state...", nameof(State));
                var test = JsonSerializer.Deserialize<StateSnapshot>(File.ReadAllText(pathJoin), Options);
                if (test is null)
                {
                    throw new JsonException(message:"Whole file is null. ");
                }
                LauncherLogger.LogInfo("Validated. ", nameof(State));
            }
            catch (JsonException e)
            {
                LauncherLogger.LogError("Error reading state file: \n" + e, nameof(State));
                LauncherLogger.LogError("Deleting existing state file and replace with a new one.", nameof(State));
                File.Delete(pathJoin);
                Recursive();
            }
        }

        Recursive();
    }
    public static void Set<T>(Expression<Action<StateSnapshot>> expression, T value) 
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
    public required bool Online { get; set; } = false; 
}