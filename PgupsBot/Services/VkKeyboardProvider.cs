using System.Collections.Concurrent;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PgupsBot.Models;
using VkNet.Enums.StringEnums;
using VkNet.Model;

namespace PgupsBot.Services;

public class VkKeyboardProvider
{
    private static IDictionary<string, MessageKeyboard>? _keyboards;
    private static LabelProvider? _labelProvider;
    
    public VkKeyboardProvider()
    {
        _labelProvider ??= new LabelProvider();
        _keyboards ??= MakeKeyboard("ru");
    }

    private IDictionary<string, MessageKeyboard> MakeKeyboard(string lang)
    {
        MessageKeyboardButton BackButton() => NewBackButton();
        MessageKeyboardButton InfoHomeButton() => NewHomeButton("start/info");
        List<MessageKeyboardButton> InfoBackHomeButtons() => new() { BackButton(), InfoHomeButton() };

        var keyboard = new ConcurrentDictionary<string, MessageKeyboard> {
            ["start"] = new () {
                Buttons = new List<List<MessageKeyboardButton>>() {
                    new () {
                        NewMenuButton("info"),
                    }, new () {
                        NewInfoButton("about"),
                    }
                }
            },
            ["start/info"] = new () {
                Buttons = new List<List<MessageKeyboardButton>> {
                    new () {
                        NewMenuButton("abit")
                    }, new () {
                        NewMenuButton("stud"),
                        NewMenuButton("prof")
                    }, new () {
                        BackButton()
                    }
                }
            },
            ["start/info/abit"] = new () {
                Buttons = new List<List<MessageKeyboardButton>> {
                    new() {
                        NewMenuButton("dorm"),
                        NewInfoButton("3d")
                    }, 
                    new() {
                        NewInfoButton("docs"),
                        NewInfoButton("contacts")
                    }, new() {
                        NewInfoButton("rules"),
                        NewInfoButton("calendar"),
                    },new() {
                        NewInfoButton("exam"),
                    },
                    InfoBackHomeButtons()
                }
            },
            ["start/info/abit/dorm"] = new () {
                Buttons = new List<List<MessageKeyboardButton>> {
                    new() {
                        NewInfoButton("d1"),
                        NewInfoButton("d2"),
                        NewInfoButton("d3"),
                    }, new() {
                        NewInfoButton("d4"),
                        NewInfoButton("d6"),
                        NewInfoButton("d8"),
                    }, new() {
                        NewInfoButton("gork")
                    },
                    InfoBackHomeButtons()
                }
            },
            ["start/info/stud"] = new () {
                Buttons = new List<List<MessageKeyboardButton>>() {
                    new() {
                        NewInfoButton("struct"),
                        NewInfoButton("map"),
                    },new() {
                        NewInfoButton("scholarship"),
                        NewInfoButton("portfolio"),
                    },  new() {
                        NewInfoButton("infrastructure"),
                    }, new() {
                        NewMenuButton("facs"),
                        NewMenuButton("dorm")
                    }, 
                    
                    InfoBackHomeButtons(),
                }
            },
            ["start/info/stud/facs"] = new () {
                Buttons = new List<List<MessageKeyboardButton>>() {
                    new() {
                        NewInfoButton("ait"),
                        NewInfoButton("pgs"),
                    }, new() {
                        NewInfoButton("ts"),
                        NewInfoButton("tes"),
                    }, new() {
                        NewInfoButton("upl"),
                        NewInfoButton("eim"),
                    }, new() {
                        NewInfoButton("bfo"),
                    },
                    InfoBackHomeButtons()
                }
            },
            ["start/info/stud/dorm"] = new() {
                Buttons = new List<List<MessageKeyboardButton>> {
                    new() {
                        NewInfoButton("d1"),
                        NewInfoButton("d2"),
                        NewInfoButton("d3"),
                    }, new() {
                        NewInfoButton("d4"),
                        NewInfoButton("d5"),
                        NewInfoButton("d5k3"),
                    }, new() {
                        NewInfoButton("d6"),
                        NewInfoButton("d7a"),
                        NewInfoButton("d8"),
                    },
                    InfoBackHomeButtons()
                }
            },
            // ["start/info/stud/infrastructure"] = new() {
            //     Buttons = new List<List<MessageKeyboardButton>>() {
            //         new () {
            //             NewInfoButton("canteens")
            //         }, new () {
            //             NewInfoButton("libraries")
            //         }, new () {
            //             NewInfoButton("hospital")
            //         },
            //         InfoBackHomeButtons()
            //     }
            // },
            ["start/info/prof"] = new () {
                Buttons = new List<List<MessageKeyboardButton>> {
                    new() {
                        NewInfoButton("docs"),
                    },
                    InfoBackHomeButtons()
                }
            },
        };
        
        SetMenus(keyboard);
        SetLabels(keyboard, lang);

        return keyboard;
    }
    
    private static MessageKeyboardButton NewBackButton() => new ()
    {
        Action = new MessageKeyboardButtonAction()
        {
            Type = KeyboardButtonActionType.Text,
            Label = "[Back]",
            Payload = new Payload("back", "", "[CurrentMenu]").ToJson(),
        },
        Color = KeyboardButtonColor.Secondary
    };
    
    private static MessageKeyboardButton NewHomeButton(string homeMenu) => new ()
    {
        Action = new MessageKeyboardButtonAction()
        {
            Type = KeyboardButtonActionType.Text,
            Label = "[Home]",
            Payload = new Payload("set", homeMenu, "[CurrentMenu]").ToJson(),
        },
        Color = KeyboardButtonColor.Secondary
    };

    private static MessageKeyboardButton NewMenuButton(string name) => new()
    {
        Action = new MessageKeyboardButtonAction()
        {
            Type = KeyboardButtonActionType.Text,
            Label = "[Label]",
            Payload = new Payload("select", name, "[CurrentMenu]").ToJson()
        },
        Color = KeyboardButtonColor.Primary,
    };
    
    private static MessageKeyboardButton NewInfoButton(string name) => new()
    {
        Action = new MessageKeyboardButtonAction()
        { 
            Type = KeyboardButtonActionType.Text,
            Label = "[Label]",
            Payload = new Payload("display", name, "[CurrentMenu]").ToJson(),
        },
        Color = KeyboardButtonColor.Primary,
    };

    private static void SetMenus(IDictionary<string, MessageKeyboard> keyboard)
    {
        foreach (var pair in keyboard)
        {
            SetMenu(pair.Key, pair.Value);    
        }
    }

    private static void SetMenu(string menu, MessageKeyboard keyboard)
    {
        foreach (var line in keyboard.Buttons)
        {
            foreach (var button in line)
            {
                button.Action.Payload = button.Action.Payload.Replace("[CurrentMenu]", menu);
            }
        }
    }

    private void SetLabels(IDictionary<string, MessageKeyboard> keyboard, string lang)
    {
        foreach (var pair in keyboard)
        {
            SetLabel(pair.Key, pair.Value, lang);    
        }
    }

    private void SetLabel(string path, MessageKeyboard keyboard, string lang)
    {
        foreach (var line in keyboard.Buttons)
        {
            foreach (var button in line)
            {
                string? arg = JsonConvert.DeserializeObject<JObject>(button.Action.Payload)?["arg"]?.ToString();
                button.Action.Label = button.Action.Label switch
                {
                    "[Label]" => _labelProvider!.GetLabel($"{path}/{arg}"),
                    "[Back]" => _labelProvider!.GetLabel("back"),
                    "[Home]" => _labelProvider!.GetLabel("home"),
                    _ => path,
                };
            }
        }
    }

    public MessageKeyboard? GetKeyboard(string menu) => 
        _keyboards!.TryGetValue(menu, out var value) ? value : null;
}