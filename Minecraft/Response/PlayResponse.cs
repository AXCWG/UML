namespace Minecraft.Response;

class PlayResponse : Response
{
    public PlayResponse(long id) : base(id)
    {
        Type = Request.Request.MessageType.Play;
    }
}