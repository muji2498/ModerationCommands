using System.Linq;
using CommandMod;
using CommandMod.CommandHandler;
using CommandMod.Extensions;
using Moderation.Handlers;
using NuclearOption.Networking;

namespace Moderation.Commands;

public class PlayerCommands
{
    [ConsoleCommand("kick", Roles.Owner | Roles.Admin | Roles.Moderator)]
    public static void KickPlayer(string[] args, CommandObjects arg2)
    {
        if (args.Length < 1)
        {
            arg2.Player.SendChatMessage("Usage: [kick playername|steamid]");
            return;
        }
        var callingPlayer = arg2.Player;
        var targetPlayer = args[0];
        var playersToKick = CommandMod.Utils.IdentifyPlayer(targetPlayer);
        if (playersToKick == null || playersToKick.Count == 0)
        {
            arg2.Player.SendChatMessage("Player not found.");
            return;
        }

        if (playersToKick.Count == 1)
        {
            var playerToKick = playersToKick.First();
            NetworkManagerNuclearOption.i.KickPlayerAsync(playerToKick);
            arg2.Player.SendChatMessage($"Player {playerToKick.PlayerName} kicked.");
            return;
        }
        
        PlayerModerationHandler.AddToSelection(callingPlayer,"kick", playersToKick);
    }
    
    [ConsoleCommand("ban", Roles.Owner | Roles.Admin | Roles.Moderator)]
    public static void Ban(string[] args, CommandObjects context)
    {
        if (args.Length < 1)
        {
            context.Player.SendChatMessage("Usage: [ban playername|steamid]");
            return;
        }

        var callingPlayer = context.Player;
        var targetPlayer = args[0];
        
        var playersToBan = CommandMod.Utils.IdentifyPlayer(targetPlayer);
        if (playersToBan == null || playersToBan.Count == 0)
        {
            context.Player.SendChatMessage("Player not found.");
            return;
        }

        if (playersToBan.Count == 1)
        {
            var playerToBan = playersToBan.First();
            PlayerModerationHandler.BanPlayer(playerToBan.SteamID, callingPlayer);
            return;
        }
        
        PlayerModerationHandler.AddToSelection(callingPlayer, "ban", playersToBan);
    }
    
    [ConsoleCommand("unban",  Roles.Owner | Roles.Admin | Roles.Moderator)]
    public static void Unban(string[] args, CommandObjects context)
    {
        if (args.Length < 1)
        {
            context.Player.SendChatMessage("Usage: [unban steamid]");
            return;
        }

        var callingPlayer = context.Player;
        var targetPlayer = args[0];

        if (!ulong.TryParse(targetPlayer, out var steamId))
        {
            context.Player.SendChatMessage("Please use a numeric value.");
            return;
        }
        PlayerModerationHandler.UnbanPlayer(steamId, callingPlayer);
    }

    [ConsoleCommand("select", Roles.Owner | Roles.Admin | Roles.Moderator)]
    public static void Select(string[] args, CommandObjects context)
    {
        var callingPlayer = context.Player;
        if (args.Length < 1)
        {
            context.Player.SendChatMessage("Usage: [select number|cancel]");
            return;
        }

        lock (PlayerModerationHandler.PendingSelections)
        {
            if (!PlayerModerationHandler.PendingSelections.TryGetValue(callingPlayer, out var pendingSelection))
            {
                context.Player.SendChatMessage("No selections. Use [kick|ban] commands first.");
                return;
            }
            
            var argument = args[0];
            if (argument == "cancel")
            {
                PlayerModerationHandler.PendingSelections.Remove(callingPlayer);
                context.Player.SendChatMessage("Selection cancelled.");
                return;
            }

            if (!int.TryParse(argument, out int number) || number < 0 || number > pendingSelection.Players.Count)
            {
                // number passed in
                context.Player.SendChatMessage("Invalid selection number.");
                return;
            }

            var playerToSelect = pendingSelection.Players[number - 1];
            switch (pendingSelection.SelectionType.ToLower())
            {
                case "kick":
                    NetworkManagerNuclearOption.i.KickPlayerAsync(playerToSelect);
                    context.Player.SendChatMessage($"Player {playerToSelect.PlayerName} kicked.");
                    break;
                case "ban":
                    PlayerModerationHandler.BanPlayer(playerToSelect.SteamID, callingPlayer);
                    break;
            }

            PlayerModerationHandler.PendingSelections.Remove(callingPlayer);
        }
    }
}