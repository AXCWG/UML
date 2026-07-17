using System.Text.Json;
using System.Text.Json.Serialization;
using Photino.NET;
namespace Minecraft;
class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        try
        {
            var window = new PhotinoWindow()
                .SetTitle("Launcher")
                .SetSize(1024, 768)
                .Center()
                .SetDevToolsEnabled(true)
                .RegisterWebMessageReceivedHandler((sender, message) =>
                {
                    Request? obj;
                    try
                    {
                        obj = JsonSerializer.Deserialize<Request>(message, options: JsonSerializerOptions.Web);
                        
                        switch (obj?.Type)
                        {
                            case Request.MessageType.Message:
                                try
                                {
                                    MessageRequest? messageRequest =
                                        JsonSerializer.Deserialize<MessageRequest>(message, JsonSerializerOptions.Web);
                                    ((PhotinoWindow)sender!).SendWebMessage(JsonSerializer.Serialize<MessageResponse>(
                                        new()
                                        {
                                            Id = obj.Id,
                                            Type = Request.MessageType.Message,
                                            Content = $"Got it: {messageRequest?.Content}"
                                        }));
                                }
                                catch (Exception e)
                                {
                                    ((PhotinoWindow)sender!).SendErrorMessage(new()
                                    {
                                        Id =  obj.Id,
                                        Type = Request.MessageType.Error,
                                        Error = e.ToString()
                                    });
                                }
                                
                                break;
                            case Request.MessageType.Error:
                                ErrorRequest? errorRequest = JsonSerializer.Deserialize<ErrorRequest>(message, JsonSerializerOptions.Web);
                                Console.WriteLine("{0}[Frontend] {1}", DateTime.Now, errorRequest?.Error);
                                break;
                            case Request.MessageType.Addition:
                                try
                                {
                                    AdditionRequest? additionRequest =
                                        JsonSerializer.Deserialize<AdditionRequest>(message, JsonSerializerOptions.Web);
                                    ((PhotinoWindow)sender!).SendWebMessage(JsonSerializer.Serialize<AdditionResponse>(
                                        new()
                                        {
                                            Id = obj.Id,
                                            Type = Request.MessageType.Addition,
                                            Result = additionRequest.A + additionRequest.B
                                        }));
                                }
                                catch (Exception e)
                                {
                                    ((PhotinoWindow)sender!).SendWebMessage(JsonSerializer.Serialize<ErrorResponse>(
                                        new()
                                        {
                                            Id = obj.Id,
                                            Type = Request.MessageType.Error,
                                            Error = e.ToString()
                                        }));
                                }
                                
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
                    
                });

#if DEBUG
            Console.WriteLine("Loading from Vite dev server (http://localhost:3000)...");
            window.Load(new Uri("http://localhost:3000"));
#else
    Console.WriteLine("Loading from wwwroot/index.html...");
    window.Load("wwwroot/index.html");
#endif

            window.WaitForClose();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[Fatal] {ex}");
        }

    }
}

record Request
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum MessageType
    {
        [JsonStringEnumMemberName("message")]
        Message,
        [JsonStringEnumMemberName("error")]
        Error,
        [JsonStringEnumMemberName("addition")]
        Addition,
    }
    public long Id { get; set; }
    public MessageType? Type { get; set; } 
    
}

record MessageRequest : Request
{
    public string? Content { get; set; }
}

record AdditionRequest : Request
{
    public int A { get; set; }
    public int B { get; set; }
}

record ErrorRequest : Request
{
    public string Error { get; set; }
}

class Response
{
    public long Id { get; set; }
    public Request.MessageType Type { get; set; }
    
}

class  MessageResponse : Response
{
    public string? Content { get; set; }   
}

class AdditionResponse : Response
{
    public long Result { get; set; }
}

class ErrorResponse : Response
{
    public required string Error { get; set; }
}


static class PhotinoExtension
{
    extension(PhotinoWindow window)
    {
        public void SendErrorMessage(ErrorResponse e)
        {
            window.SendWebMessage(JsonSerializer.Serialize<ErrorResponse>(e));
        }
    }
}