namespace Minecraft.Request;

record MessageRequest : Request
{
    public string? Content { get; set; }
}