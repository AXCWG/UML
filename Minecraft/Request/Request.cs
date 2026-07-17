using System.Text.Json.Serialization;

namespace Minecraft.Request;

record Request
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MessageType
    {
        [JsonStringEnumMemberName("message")]
        Message,
        [JsonStringEnumMemberName("error")]
        Error,
        [JsonStringEnumMemberName("addition")]
        Addition,
        [JsonStringEnumMemberName("getConfig")]
        GetConfig,
        [JsonStringEnumMemberName("setConfig")]
        SetConfig,
    }
    public long Id { get; set; }
    public MessageType? Type { get; set; } 
    
}