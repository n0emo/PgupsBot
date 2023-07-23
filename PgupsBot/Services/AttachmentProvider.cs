using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using VkNet.Abstractions;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;

namespace PgupsBot.Services;

public class AttachmentProvider
{
    private static IDictionary<string, string>? _photos;
    private readonly IVkApi _vkApi;

    public AttachmentProvider(IVkApi vkApi)
    {
        _vkApi = vkApi;
        _photos ??= MakePhotos();
    }

    private static IDictionary<string, string> MakePhotos()
    {
        try
        {
            var photosJson = File.ReadAllText(@"Resources/photos_ru.json");
            var photos = JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(photosJson)!;
            return photos;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<IEnumerable<MediaAttachment>?> Get(string path)
    {
        if (!_photos!.TryGetValue(path, out var pathToPhoto))
        {
            return null;
        }
        
        var content = new MultipartFormDataContent();
        var pathToFile = $@"{Directory.GetCurrentDirectory()}/Resources/Photos/{pathToPhoto}";
        var fileStreamContent = new StreamContent(File.OpenRead(pathToFile));
        fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
        content.Add(fileStreamContent, name: "file", fileName: $"{DateTime.Now.Millisecond}-{pathToPhoto}.jpg");
        
        try
        {
            using var httpClient = new HttpClient();
            var uploadServer = await _vkApi.Photo.GetMessagesUploadServerAsync(_vkApi.UserId!.Value);
            var response = await httpClient.PostAsync(uploadServer.UploadUrl, content);
            response.EnsureSuccessStatusCode();
            var responseFile = await response.Content.ReadAsStringAsync();
            var photos = _vkApi.Photo.SaveMessagesPhoto(responseFile);
            return photos;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
}