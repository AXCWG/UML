namespace Minecraft.Response;

class AddProfileResponse:Response
{
    public AddProfileResponse(long id) : base(id)
    {
        Type = Request.Request.MessageType.AddProfile;
    }
}