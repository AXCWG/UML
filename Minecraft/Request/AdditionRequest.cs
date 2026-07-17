namespace Minecraft.Request;

record AdditionRequest : Request
{
    public int A { get; set; }
    public int B { get; set; }
}