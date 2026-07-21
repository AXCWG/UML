namespace Minecraft.Response;

class AdditionResponse(long id, long result) : Response(id)
{
    public long Result { get; set; } = result;
}