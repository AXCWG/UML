namespace Minecraft.Request;

record AddProfileRequest : Request
{
    public string? Name { get; set; }
}