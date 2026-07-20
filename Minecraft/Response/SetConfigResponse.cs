namespace Minecraft.Response;

class SetConfigResponse  : Response
{
    public SetConfigResponse(long id) : base(id)
    {
        Type = Request.Request.MessageType.SetConfig;
    }
}