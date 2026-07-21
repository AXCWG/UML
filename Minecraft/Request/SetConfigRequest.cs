namespace Minecraft.Request;

record SetConfigRequest : Request
{
    public StateSnapshot? Snapshot { get; set; }
    public static implicit operator StateSnapshot(SetConfigRequest request)
    {
        return request.Snapshot ?? throw new NullReferenceException("Snapshot is null");
    } 
}