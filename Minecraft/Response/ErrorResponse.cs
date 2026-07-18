namespace Minecraft.Response;

class ErrorResponse : Response
{
    public ErrorResponse(long id, string error)
    {
        Id = id; 
        Error = error;
        Type = Request.Request.MessageType.Error;
    }
    public  string Error { get; set; }
}