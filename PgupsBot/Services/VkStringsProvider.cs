namespace PgupsBot.Services;

public class VkStringsProvider
{
    public static string Token 
        => File.ReadAllText("Resources/VK/token.txt");
    
    public static string ConfirmationCode 
        => File.ReadAllText("Resources/VK/confirmation.txt");
    
    public static async Task<string> GetTokenAsync() 
        => await File.ReadAllTextAsync("Resources/VK/token.txt");

    public static async Task<string> GetConfirmationCodeAsync() 
        => await File.ReadAllTextAsync("Resources/VK/confirmation.txt");
}