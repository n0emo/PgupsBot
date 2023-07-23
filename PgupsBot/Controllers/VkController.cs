using Microsoft.AspNetCore.Mvc;
using VkNet.Abstractions;
using PgupsBot.Models;
using PgupsBot.Services;
using VkNet.Model;

namespace PgupsBot.Controllers;

[ApiController]
[Route("[Controller]")]
public class VkController : ControllerBase
{

    private readonly IVkApi _vkApi;
    private readonly ResponseMessageProvider _responseProvider;

    public VkController(IVkApi vkApi, ResponseMessageProvider responseProvider)
    {
        _vkApi = vkApi;
        _responseProvider = responseProvider;
    }

    [HttpPost]
    public async Task<IActionResult> Callback([FromBody] Update update)
    {
        if (update.Secret is not "d68HjdH7024Kdf3")
        {
            return Unauthorized("Invalid secret");
        }

        return update.Type switch
        {
            "confirmation" => Ok(await VkStringsProvider.GetConfirmationCodeAsync()),
            "message_new" => await MessageNew(update),
            _ => Ok("ok"),
        };
    }

    private async Task<IActionResult> MessageNew(Update update)
    {
        if (update.Object is null)
        {
            return Problem("Object required.");
        }

        if (update.Object["message"] is null)
        {
            return Problem("Message object required.");
        }

        var message = update.Object["message"]!.ToObject<Message>()!;

        var response = message.Payload switch
        {
            null => await _responseProvider.GetDefaultResponse(message),
            _ => await _responseProvider.GetResponse(message),
        };

        if (response is null)
            return Problem("Error getting response");
                
        await _vkApi.Messages.SendAsync(response);
        return Ok("ok");
    }
}