using System.Text.Json;
using AXExpansion;
using Minecraft.Request;
using Minecraft.Response;
using Photino.NET;

namespace Minecraft;
class Program
{
    [STAThread]
    static void Main(string[] args)
    {
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
                        obj = JsonSerializer.Deserialize<Request.Request>(message, options: JsonSerializerOptions.Web);

                        var photinoWindow = ((PhotinoWindow)sender!);
                        switch (obj?.Type)
                        {
                            case Request.Request.MessageType.Message:
                                try
                                {
                                    MessageRequest? messageRequest =
                                        JsonSerializer.Deserialize<MessageRequest>(message, JsonSerializerOptions.Web);
                                    await photinoWindow.SendWebMessageAsync(JsonSerializer.Serialize<MessageResponse>(
                                        new()
                                        {
                                            Id = obj.Id,
                                            Type = Request.Request.MessageType.Message,
                                            Content = $"Got it: {messageRequest?.Content}"
                                        }, JsonSerializerOptions.Web));
                                }
                                catch (Exception e)
                                {
                                    photinoWindow.SendErrorMessage(new(obj.Id, e.ToString()));
                                }
                                
                                break;
                            case Request.Request.MessageType.Error:
                                ErrorRequest? errorRequest = JsonSerializer.Deserialize<ErrorRequest>(message, JsonSerializerOptions.Web);
                                Console.WriteLine("{0}[Frontend] {1}", DateTime.Now, errorRequest?.Error);
                                break;
                            case Request.Request.MessageType.Addition:
                                try
                                {
                                    AdditionRequest? additionRequest =
                                        JsonSerializer.Deserialize<AdditionRequest>(message, JsonSerializerOptions.Web);
                                    await photinoWindow.SendWebMessageAsync(JsonSerializer.Serialize<AdditionResponse>(
                                        new()
                                        {
                                            Id = obj.Id,
                                            Type = Request.Request.MessageType.Addition,
                                            Result = additionRequest.A + additionRequest.B
                                        }, JsonSerializerOptions.Web));
                                }
                                catch (Exception e)
                                {
                                    await photinoWindow.SendWebMessageAsync(JsonSerializer.Serialize<ErrorResponse>(
                                        new(obj.Id, e.ToString()), JsonSerializerOptions.Web));
                                }
                                
                                break;
                            case Request.Request.MessageType.GetConfig:
                                await State.CheckVersion();
                                await photinoWindow.SendWebMessageAsync(JsonSerializer.Serialize(new ConfigResponse
                                {
                                    Id = obj.Id,
                                    Type = Request.Request.MessageType.GetConfig,
                                    Snapshot = State.GetAll
                                }, JsonSerializerOptions.Web));
                                break;
                            case Request.Request.MessageType.SetConfig:
                                //TODO
                                var sCReq =
                                    JsonSerializer.Deserialize<SetConfigRequest>(message, JsonSerializerOptions.Web);
                                if (sCReq?.Snapshot is null)
                                {
                                    LauncherLogger.LogError<Program>("Client request set state failed: null request. ");
                                    await photinoWindow.SendErrorMessageAsync(new(id: obj.Id, error: "State config request is null. "));
                                    break;
                                }
                                State.Set(sCReq);
                                await photinoWindow.SendWebMessageAsync(JsonSerializer.Serialize(new SetConfigResponse(obj.Id)));
                                break;
                            case Request.Request.MessageType.Play:
                                LauncherLogger.LogWarning<Program>("Launch clicked. ");
                                State.LauncherBackend.Play().GetAwaiter().GetResult();
                                await photinoWindow.SendWebMessageAsync(JsonSerializer.SerializeWeb(new PlayResponse(obj.Id)));
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
                }).SetFileSystemAccessEnabled(true);

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