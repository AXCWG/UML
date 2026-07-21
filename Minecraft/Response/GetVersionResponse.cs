// namespace Minecraft.Response;
//
// class GetVersionResponse : Response
// {
//     public GetVersionResponse(long id)
//     {
//         Id = id;
//         Type = Request.Request.MessageType.GetVersions;
//         State.CheckVersion().GetAwaiter().GetResult();
//         Profiles = State.Get(i => i.Profiles);
//     }
//     public List<StateSnapshot.Profile>? Profiles { get; set; }
// }