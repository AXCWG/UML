namespace Minecraft.Response;

 class ConfigResponse : Response
{
    public required StateSnapshot Snapshot { get; set; } 
}