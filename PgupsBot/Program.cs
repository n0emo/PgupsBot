using VkNet;
using VkNet.Abstractions;
using PgupsBot.Services;
using VkNet.Model;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddNewtonsoftJson(jsonOptions => { });

builder.Services.AddScoped<IVkApi, VkApi>(provider =>
{
    var vkApi = new VkApi();
    vkApi.Authorize(new ApiAuthParams()
    {
        AccessToken = VkStringsProvider.GetTokenAsync().Result
    });
    return vkApi;
});

builder.Services.AddScoped<ResponseMessageProvider, ResponseMessageProvider>();

builder.Services.AddScoped<AttachmentProvider, AttachmentProvider>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();