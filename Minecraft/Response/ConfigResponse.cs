namespace Minecraft.Response;

 class ConfigResponse : Response
{
    public ConfigResponse(long id, StateSnapshot snapshot) : base(id)
    {
        Type = Request.Request.MessageType.SetConfig;
        Snapshot = snapshot;
    }
    public  StateSnapshot Snapshot { get; set; } 
}