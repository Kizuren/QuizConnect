using System.Text;
using System.Text.Json;

namespace WritingServer.Services;

public interface IDiscordNotificationService
{
    Task SendStartupNotification(TimeSpan startupTime, long memoryUsageMb);
    Task SendResponseNotification(string username, string questionSetName,
        Dictionary<string, string> response);
    Task SendExceptionNotification(string path);
    Task SendDiscordMessage(string content, string username);
}

public class DiscordNotificationService : IDiscordNotificationService
{
    private readonly HttpClient _httpClient;
    private readonly string _webhookUrl;

    public DiscordNotificationService()
    {
        _httpClient = new HttpClient();
        _webhookUrl = Environment.GetEnvironmentVariable("DISCORD_WEBHOOK") ?? "";
    }

    public async Task SendStartupNotification(TimeSpan startupTime, long memoryUsageMb)
    {
        await SendDiscordMessage($"Backend warmed up!\nTook {startupTime.TotalSeconds:F2} seconds.\nMemory usage: {memoryUsageMb} MB");
    }

    public async Task SendResponseNotification(string username, string questionSetName,
        Dictionary<string, string> response)
    {
        var message = $"**{questionSetName}**\n**Question:**\n";
        foreach (var item in response)
        {
            message += $"**{item.Key}** {item.Value}\n";
        }
        await SendDiscordMessage(message, username);
    }
    
    public async Task SendExceptionNotification(string path)
    {
        var message = $"An unhandled exception occurred at path: {path}\nMore details can be in found in `stdout`";
        await SendDiscordMessage(message);
    }

    public async Task SendDiscordMessage(string content, string? username = null)
    {
        if (string.IsNullOrEmpty(_webhookUrl))
        {
            return;
        }

        object payload;
        if (!string.IsNullOrEmpty(username))
        {
            payload = new { content, username };
        }
        else
        {
            payload = new { content };
        }
        var json = JsonSerializer.Serialize(payload);
        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            await _httpClient.PostAsync(_webhookUrl, stringContent);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send Discord notification: {ex.Message}");
        }
    }
}