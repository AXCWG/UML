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
            window.SendWebMessage(JsonSerializer.Serialize(e, JsonSerializerOptions.Web));
        }

        public async Task SendErrorMessageAsync(ErrorResponse e)
        {
            await window.SendWebMessageAsync(JsonSerializer.Serialize(e, JsonSerializerOptions.Web));
        }
    }

    extension(JsonSerializer)
    {
        public static string SerializeWeb<T>(T obj)
        {
            return JsonSerializer.Serialize(obj, JsonSerializerOptions.Web);
        }

        public static T? DeserializeWeb<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, JsonSerializerOptions.Web);
        }
    }
}