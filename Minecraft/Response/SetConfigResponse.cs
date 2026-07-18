namespace Minecraft.Response;

class SetConfigResponse  : Response
{
    public SetConfigResponse(long id)
    {
        this.Id = id;
        Type = Request.Request.MessageType.SetConfig;
    }
}