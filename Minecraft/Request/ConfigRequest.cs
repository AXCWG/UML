namespace Minecraft.Request;

record SetConfigRequest
{
    public bool Online { get; set; }
}