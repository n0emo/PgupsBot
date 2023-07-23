using Newtonsoft.Json;

namespace PgupsBot.Models;

[Serializable]
public class Payload
{
    [JsonProperty("command")]
    public string Command { get; set; }
    
    [JsonProperty("current_menu")]
    public string CurrentMenu { get; set; }
    
    [JsonProperty("arg")]
    public string Arg { get; set; }

    public Payload(string command, string arg, string currentMenu)
    {
        Command = command;
        CurrentMenu = currentMenu;
        Arg = arg;
    }

    public string ToJson()
    {
        return JsonConvert.SerializeObject(this, Formatting.None);
    }
}