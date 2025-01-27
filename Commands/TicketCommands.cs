using CommandMod;
using CommandMod.CommandHandler;
using Moderation.Handlers;
using Moderation.Utils;

namespace Moderation.Commands;

public class TicketCommands
{
    [ConsoleCommand("ticket")]
    public static void Ticket(string[] args, CommandObjects objects)
    {
        var callingPlayer = objects.Player;
        
        if (args.Length < 1)
        {
            Wrapper.ChatManager.TargetReceiveMessage(callingPlayer.Owner, "Usage: ticket <message>", callingPlayer, false);
            return;
        }

        var message = string.Join(" ", args);
        message = DiscordUtils.Sanitise(message);

        var steamId = PlayerUtils.GetSteamId(callingPlayer);
        message = $"{callingPlayer.PlayerName} ({steamId}): {message}";
        
        if (!string.IsNullOrEmpty(ModerationPlugin.Config.MessagePrefix.Value))
        {
            message = $"{ModerationPlugin.Config.MessagePrefix.Value} {message}";
        }
        
        ModerationPlugin.Logger.LogInfo($"Sending Ticket: {message} from {callingPlayer.PlayerName}");
        Wrapper.ChatManager.TargetReceiveMessage(callingPlayer.Owner, "Ticket sent!", callingPlayer, false);

        DiscordWebhookHandler.SendToWebhook(message);
    }
}