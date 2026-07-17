namespace Minecraft.Request;

record ErrorRequest : Request
{
    public string? Error { get; set; }
}