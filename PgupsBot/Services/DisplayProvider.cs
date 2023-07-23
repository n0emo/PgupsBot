using System.Collections.Concurrent;
using Newtonsoft.Json;

namespace PgupsBot.Services;

public class DisplayProvider
{
    private static IDictionary<string, IDictionary<string, string>>? _displays;
    private readonly string _defaultLang;

    public DisplayProvider(string defaultLang = "ru")
    {
        _defaultLang = defaultLang;
        
        if(_displays is null)
        {
            _displays = new ConcurrentDictionary<string, IDictionary<string, string>>();

            var paths = Directory.GetFiles("Resources/").Where(s => s.Contains("displays"));
            foreach (var path in paths)
            {
                try
                {
                    string lang = path.Substring(
                        path.IndexOf("displays_", StringComparison.Ordinal) + "displays_".Length,
                        2);
                    string json = File.ReadAllText(path);
                    var displays = JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(json)!;
                    _displays.Add(lang, displays);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }

    public string GetDisplay(string path, string? lang = null)
    {
        lang ??= _defaultLang;
        if (_displays!.ContainsKey(lang) && _displays[lang].ContainsKey(path))
        {
            return _displays[lang][path];
        }
        if (_defaultLang.Contains(lang) && !_displays[lang].ContainsKey(path))
        {
            return _displays[_defaultLang].TryGetValue(path, out var value) ? value : path;
        }
        return path;
    }
}