namespace Minecraft.Response;

class ErrorResponse : Response
{
    public required string Error { get; set; }
}