namespace Minecraft.Response;

class ErrorResponse : Response
{
    public ErrorResponse(long id, string error) : base(id)
    {
        Error = error;
        Type = Request.Request.MessageType.Error;
    }
    public  string Error { get; set; }
}