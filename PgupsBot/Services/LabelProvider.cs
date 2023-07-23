using System.Collections.Concurrent;
using Newtonsoft.Json;

namespace PgupsBot.Services;

public class LabelProvider
{
    private static IDictionary<string, string>? _labelsRu = null;

    public LabelProvider(string defaultLang = "ru")
    {
        _labelsRu ??= MakeLabels();
    }

    private static IDictionary<string, string> MakeLabels()
    {
        try
        {
            var jsonRu = File.ReadAllText(@"Resources/labels_ru.json");
            var labelsRu = JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(jsonRu)!;
            return labelsRu;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public string GetLabel(string path, string lang = "ru") =>
        _labelsRu!.TryGetValue(path, out var value) ? value : path;
    
}