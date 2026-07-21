using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using AXExpansion;
using Minecraft.Request;
using Minecraft.Response;
using Photino.NET;

namespace Minecraft;
class Program
{
    [DllImport("kernel32.dll")]
    private static extern bool AllocConsole();

    [STAThread]
    static void Main(string[] args)
    {
        if (args.Contains("--logwindow"))
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                AllocConsole();
        }

        LauncherLogger.LogInfo<Program>("Program starting. ");
        try
        {
            var window = new PhotinoWindow()
                .SetTitle("Launcher")
                .SetSize(1024, 768)
                .Center()
                .SetDevToolsEnabled(true)
                .SetContextMenuEnabled(false)
                .RegisterWebMessageReceivedHandler(async void (sender, message) =>
                {
                    Request.Request? obj;
                    try
                    {
                        obj = JsonSerializer.Deserialize(message, UmlWebJsonContext.Default.Request);

                        var photinoWindow = ((PhotinoWindow)sender!);
                        switch (obj?.Type)
                        {
                            case Request.Request.MessageType.Message:
                                try
                                {
                                    MessageRequest? messageRequest =
                                        JsonSerializer.Deserialize(message, UmlWebJsonContext.Default.MessageRequest);
                                    await photinoWindow.SendWebMessageAsync(JsonSerializer.Serialize(
                                        new MessageResponse(obj.Id, $"Got it: {messageRequest?.Content}"),
                                        UmlWebJsonContext.Default.MessageResponse));
                                }
                                catch (Exception e)
                                {
                                    await photinoWindow.SendErrorMessageAsync(new(obj.Id, e.ToString()));
                                }
                                
                                break;
                            case Request.Request.MessageType.Error:
                                ErrorRequest? errorRequest = JsonSerializer.Deserialize(message, UmlWebJsonContext.Default.ErrorRequest);
                                Console.WriteLine("{0}[Frontend] {1}", DateTime.Now, errorRequest?.Error);
                                break;
                            case Request.Request.MessageType.Addition:
                                try
                                {
                                    AdditionRequest? additionRequest =
                                        JsonSerializer.Deserialize(message, UmlWebJsonContext.Default.AdditionRequest);
                                    await photinoWindow.SendWebMessageAsync(JsonSerializer.Serialize(
                                        new AdditionResponse(obj.Id, additionRequest.A + additionRequest.B),
                                        UmlWebJsonContext.Default.AdditionResponse));
                                }
                                catch (Exception e)
                                {
                                    await photinoWindow.SendWebMessageAsync(JsonSerializer.Serialize(
                                        new ErrorResponse(obj.Id, e.ToString()),
                                        UmlWebJsonContext.Default.ErrorResponse));
                                }
                                
                                break;
                            case Request.Request.MessageType.GetConfig:
                                await State.CheckVersion();
                                await photinoWindow.SendWebMessageAsync(JsonSerializer.Serialize(
                                    new ConfigResponse(obj.Id, State.GetAll),
                                    UmlWebJsonContext.Default.ConfigResponse));
                                break;
                            case Request.Request.MessageType.SetConfig:
                                //TODO
                                var sCReq =
                                    JsonSerializer.Deserialize(message, UmlWebJsonContext.Default.SetConfigRequest);
                                if (sCReq?.Snapshot is null)
                                {
                                    LauncherLogger.LogError<Program>("Client request set state failed: null request. ");
                                    await photinoWindow.SendErrorMessageAsync(new(id: obj.Id, error: "State config request is null. "));
                                    break;
                                }
                                State.Set(sCReq);
                                await photinoWindow.SendWebMessageAsync(JsonSerializer.Serialize(
                                    new SetConfigResponse(obj.Id),
                                    UmlWebJsonContext.Default.SetConfigResponse));
                                break;
                            case Request.Request.MessageType.Play:
                                LauncherLogger.LogWarning<Program>("Launch clicked. ");
                                State.LauncherBackend.Play().GetAwaiter().GetResult();
                                await photinoWindow.SendWebMessageAsync(JsonSerializer.SerializeWeb(new PlayResponse(obj.Id)));
                                break; 
                            case Request.Request.MessageType.AddProfile:
                                var addProfileRequest = JsonSerializer.Deserialize(message, UmlWebJsonContext.Default.AddProfileRequest);
                                if (addProfileRequest is null)
                                {
                                    LauncherLogger.LogError<Program>("Client request add profile failed: null request. ");
                                    await photinoWindow.SendErrorMessageAsync(new(id: obj.Id, error: "Add Profile request is null. "));
                                    break; 
                                }
                                foreach (var s in await photinoWindow.ShowOpenFolderAsync())
                                {
                                    if (!Path.Exists(s))
                                    {
                                        Directory.CreateDirectory(s);
                                    };
                                    var st = State.GetAll;
                                    st.Profiles?.Add(new()
                                    {
                                        Path = s,
                                        Name = addProfileRequest.Name,
                                        Uuid = Guid.NewGuid().ToString(),
                                        UnreliableVersionList = []
                                    });
                                    State.Set(st);
                                }
                                State.Validation();
                                await photinoWindow.SendJsonMessageAsync(new AddProfileResponse(obj.Id));
                                break; 
                            case null:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    catch(JsonException e)
                    {
                        Console.WriteLine(e);
                        return;
                    }
                    catch (Exception e)
                    {
                        throw; // TODO 处理异常
                    }
                }).SetFileSystemAccessEnabled(true).SetLogVerbosity(-1);

#if DEBUG
            Console.WriteLine("Loading from Vite dev server (http://localhost:3000)...");
            window.Load(new Uri("http://localhost:3000"));
#else
            string indexPath;
            try
            {
                indexPath = ResourceExtractor.Extract();
                Console.WriteLine($"Extracted UI to: {indexPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Embedded resource unavailable ({ex.Message}), falling back to wwwroot/");
                indexPath = Path.GetFullPath("wwwroot/index.html");
            }
            window.Load(indexPath);
#endif

            window.WaitForClose();
#if !DEBUG
            ResourceExtractor.Cleanup();
#endif
        }
        catch (Exception ex)
        {
            LauncherLogger.LogError<Program>($"{ex}");
        }
        LauncherLogger.LogInfo<Program>("Program ended. ");

    }
}
[JsonSerializable(typeof(Request.Request))]
[JsonSerializable(typeof(SetConfigRequest))]
[JsonSerializable(typeof(Request.AddProfileRequest))]
[JsonSerializable(typeof(Request.AdditionRequest))]
[JsonSerializable(typeof(Request.ErrorRequest))]
[JsonSerializable(typeof(Request.GetConfigRequest))]
[JsonSerializable(typeof(Request.MessageRequest))]
[JsonSerializable(typeof(Request.PlayRequest))]
[JsonSerializable(typeof(Response.Response))]
[JsonSerializable(typeof(Response.ConfigResponse))]
[JsonSerializable(typeof(Response.SetConfigResponse))]
[JsonSerializable(typeof(Response.AddProfileResponse))]
[JsonSerializable(typeof(Response.ErrorResponse))]
[JsonSerializable(typeof(Response.MessageResponse))]
[JsonSerializable(typeof(Response.PlayResponse))]
[JsonSerializable(typeof(Response.AdditionResponse))]
[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
internal partial class UmlWebJsonContext : JsonSerializerContext
{
}
[JsonSerializable(typeof(StateSnapshot))]
[JsonSourceGenerationOptions(WriteIndented = true, AllowTrailingCommas = true)]
internal partial class UmlStateJsonContext : JsonSerializerContext
{
}