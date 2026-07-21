namespace Minecraft.Response;

class  MessageResponse : Response
{
    public MessageResponse(long id, string? content = null) : base(id)
    {
        Type = Request.Request.MessageType.Message;
        Content = content;
    }
    public string? Content { get; set; }   
}