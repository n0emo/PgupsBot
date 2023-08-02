using Newtonsoft.Json;
using PgupsBot.Models;
using VkNet.Model;

namespace PgupsBot.Services;

public class ResponseMessageProvider
{
    private readonly VkKeyboardProvider _vkKeyboardProvider;
    private readonly DisplayProvider _displayProvider;
    private readonly AttachmentProvider _attachmentProvider;
    
    public ResponseMessageProvider(AttachmentProvider attachmentProvider)
    {
        _attachmentProvider = attachmentProvider;
        _vkKeyboardProvider = new VkKeyboardProvider();
        _displayProvider = new DisplayProvider();
    }
    
    public async Task<MessagesSendParams?> GetDefaultResponse(Message message)
    {
        return new MessagesSendParams()
        {
            DontParseLinks = true,
            Message = _displayProvider.GetDisplay("start")!,
            Keyboard = _vkKeyboardProvider.GetKeyboard("start")!,
            RandomId = Random.Shared.Next(),
            PeerId = message.PeerId,
        };
    }

    public async Task<MessagesSendParams?> GetResponse(Message message)
    {
        Payload? payload = JsonConvert.DeserializeObject<Payload>(message.Payload);
        if (payload is null) throw new Exception("Error getting payload.");

        return payload.Command switch
        {
            "start" => await StartCommand(message),
            "select" => await SelectCommand(message, payload),
            "back" => await BackCommand(message, payload),
            "display" => await DisplayCommand(message, payload),
            "set" => await SetCommand(message, payload),
            _ => await GetDefaultResponse(message),
        };
    }

    private async Task<MessagesSendParams?> StartCommand(Message message)
    {
        var response = await GetDefaultResponse(message);
        response!.Message = "Добро пожаловать в бота!";
        response.Keyboard = _vkKeyboardProvider.GetKeyboard("start")!;
        return response;
    }

    private async Task<MessagesSendParams?> SelectCommand(Message message, Payload payload)
    {
        var newMenu = $"{payload.CurrentMenu}/{payload.Arg}";
        var newKeyboard = _vkKeyboardProvider.GetKeyboard(newMenu) ??
                          _vkKeyboardProvider.GetKeyboard(payload.CurrentMenu)!;
        
        return new MessagesSendParams
        {
            DontParseLinks = true,
            Message = _displayProvider.GetDisplay(newMenu),
            Keyboard = newKeyboard,
            RandomId = Random.Shared.Next(),
            PeerId = message.PeerId,
        };
    }

    private async Task<MessagesSendParams?> BackCommand(Message message, Payload payload)
    {
        var splitPath = payload.CurrentMenu.Split('/');
        var newMenu = String.Join('/', splitPath.Take(splitPath.Length - 1));
        var newKeyboard = _vkKeyboardProvider.GetKeyboard(newMenu) ??
                          _vkKeyboardProvider.GetKeyboard(payload.CurrentMenu)!;
        
        return new MessagesSendParams()
        {
            DontParseLinks = true,
            Message = _displayProvider.GetDisplay(newMenu),
            Keyboard = newKeyboard,
            RandomId = Random.Shared.Next(),
            PeerId = message.PeerId,
        };
    }

    private async Task<MessagesSendParams?> DisplayCommand(Message message, Payload payload)
    {
        var displayMenu = $"{payload.CurrentMenu}/{payload.Arg}";

        var photos = await _attachmentProvider.Get(displayMenu);
        
        return new MessagesSendParams()
        {
            DontParseLinks = true,
            Message = _displayProvider.GetDisplay(displayMenu),
            Keyboard = _vkKeyboardProvider.GetKeyboard(payload.CurrentMenu)!,
            RandomId = Random.Shared.Next(),
            PeerId = message.PeerId,
            Attachments = photos,
        };
    }

    private async Task<MessagesSendParams?> SetCommand(Message message, Payload payload)
    {
        var newMenu = payload.Arg;
        var newKeyboard = _vkKeyboardProvider.GetKeyboard(newMenu) ??
                          _vkKeyboardProvider.GetKeyboard(payload.CurrentMenu);

        return new MessagesSendParams
        {
            DontParseLinks = true,
            Message = _displayProvider.GetDisplay(newMenu),
            Keyboard = newKeyboard,
            RandomId = Random.Shared.Next(),
            PeerId = message.PeerId,
        };
    }
}
