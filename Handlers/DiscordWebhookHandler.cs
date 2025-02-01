using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using Newtonsoft.Json;
using static System.Text.Encoding;

namespace Moderation.Handlers;

public class DiscordWebhookHandler
{
    private string DiscordWebhookUrl = "";
    
    private readonly Queue<string> Messages = new();
    private readonly object MessagesLock = new();
    
    private Timer BatchTimer;
    private const int BatchSize = 10;
    private const int BatchDelay = 2000;

    public DiscordWebhookHandler(string url)
    {
        DiscordWebhookUrl = url;
        BatchTimer = new Timer(SendBatch, null, BatchDelay, BatchDelay);
    }
    
    public void SendToWebhook(string message)
    {
        if (string.IsNullOrEmpty(DiscordWebhookUrl))
        {
            return;
        }

        lock (MessagesLock)
        {
            Messages.Enqueue(message);
        }
        
        if (Messages.Count >= BatchSize)
        {
            SendBatch(null);
        }
    }
    
    private async void SendBatch(object state)
    {
        List<string> batchMessages = null;

        lock (MessagesLock)
        {
            if (Messages.Count == 0) return;

            batchMessages = new();
            while (batchMessages.Count < BatchSize && Messages.Count > 0)
            {
                batchMessages.Add(Messages.Dequeue());
            }
        }

        if (batchMessages.Count == 0)
        {
            return;
        }
        
        var combinedMessage = string.Join("\n", batchMessages);
        var payload = new
        {
            content = combinedMessage
        };
        
        var jsonPayload = JsonConvert.SerializeObject(payload);

        using (var client = new HttpClient())
        {
            try
            {
                var response = await client.PostAsync(
                    DiscordWebhookUrl, 
                    new StringContent(jsonPayload, UTF8, "application/json")
                );

                if (response.IsSuccessStatusCode)
                {
                    ModerationPlugin.Logger.LogInfo($"Batch of {batchMessages.Count} messages sent to webhook!");
                }
                else
                {
                    ModerationPlugin.Logger.LogError($"Batch failed to send to webhook! Status: {response.StatusCode} - Reason: {response.ReasonPhrase} - Message: {response.Content.ReadAsStringAsync().Result}");
                }
            }
            catch (Exception e)
            {
                ModerationPlugin.Logger.LogError($"Error sending batch to webhook: {e.Message}");
            }
        }
    }
}