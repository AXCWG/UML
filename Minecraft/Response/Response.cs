namespace Minecraft.Response;

class Response(long id)
{
    public long Id { get; set; } = id;
    public Request.Request.MessageType Type { get; set; }
}