using System;
using System.Net.Http;
using Newtonsoft.Json;
using static System.Text.Encoding;

namespace Moderation.Handlers;

public class DiscordWebhookHandler
{
    public static async void SendToWebhook(string message)
    {
        if (string.IsNullOrEmpty(ModerationPlugin.Config.DiscordWebhook.Value))
        {
            ModerationPlugin.Logger.LogError($"DiscordWebhookHandler: No DiscordWebhook configured. Please configure it first.");
            return;
        }
        
        using (var client = new HttpClient())
        {
            var payload = new
            {
                content = message,
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);

            try
            {
                var response = await client.PostAsync(
                    ModerationPlugin.Config.DiscordWebhook.Value, 
                    new StringContent(jsonPayload, UTF8, "application/json")
                );

                if (response.IsSuccessStatusCode)
                {
                    ModerationPlugin.Logger.LogInfo($"Message: {message} sent to webhook!");
                }
                else
                {
                    ModerationPlugin.Logger.LogError($"Message: {message} failed to send to webhook! Status: {response.StatusCode} - Reason: {response.ReasonPhrase}");
                }
            }
            catch (Exception e)
            {
                ModerationPlugin.Logger.LogError($"Error sending ticket to webhook: {e.Message}");
            }
        }
    }
}