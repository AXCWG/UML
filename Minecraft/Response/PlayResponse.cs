namespace Minecraft.Response;

class PlayResponse : Response
{
    public PlayResponse(long id)
    {
        Id = id;
        Type = Request.Request.MessageType.Play;
    }
}