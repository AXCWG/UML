using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Minecraft.Response;
using Photino.NET;

namespace Minecraft;

static class PhotinoExtension
{
    extension(PhotinoWindow window)
    {
        public void SendErrorMessage(ErrorResponse e)
        {
             window.SendWebMessage(JsonSerializer.SerializeWeb(e));
        }

        public async Task SendErrorMessageAsync(ErrorResponse e)
        {
            await window.SendWebMessageAsync(JsonSerializer.SerializeWeb(e));
        }
        public async Task SendJsonMessageAsync<T>(T obj)
        {
            await window.SendWebMessageAsync(JsonSerializer.SerializeWeb(obj));
        }
    }

    extension(JsonSerializer)
    {
        public static string SerializeWeb<T>(T obj)
        {
            return JsonSerializer.Serialize(obj, UmlWebJsonContext.Default.GetTypeInfo(typeof(T))??throw new InvalidOperationException("Check source gen. "));
        }

        public static T? DeserializeWeb<T>(string json)
        {
            return (T?)JsonSerializer.Deserialize(json, UmlWebJsonContext.Default.GetTypeInfo(typeof(T))??throw new InvalidOperationException("Check source gen. "));
        }
    }
}