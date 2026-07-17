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
    }
}